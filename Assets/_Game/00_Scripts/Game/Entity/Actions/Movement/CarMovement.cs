using Game.Entities;
using UnityEngine;

public class CarMovement : MonoBehaviour, IEntityMovement
{
    public enum SpriteFacing { Up, Down, Right, Left }

    [Header("Sprite Orientation")]
    [Tooltip("Which way this car's sprite is drawn facing at rotation 0. Must match the art - different artists/sprites may face different ways.")]
    public SpriteFacing spriteFacing = SpriteFacing.Up;

    [Header("Forward-Only Firing")]
    [Tooltip("Total cone angle (degrees) in front of the car considered 'lined up' for a shot.")]
    public float forwardFireConeAngle = 20f;


    [Header("Animation")]
    [SerializeField] private string reversingBool = "IsReversing";

    private CarData carData;
    private Rigidbody2D rb;
    private EntityBrain brain;

    public float CurrentSpeed { get; private set; }
    public bool IsReversing { get; private set; }
    public Vector2 Forward => Quaternion.Euler(0f, 0f, facingOffsetDegrees) * transform.up;
    private float lastTurnSign = 1f;
    private float facingOffsetDegrees;

    public void Initialize(EntityBrain brain, EntityData carData, SpriteRenderer spriteRenderer, Rigidbody2D rb)
    {
        this.rb = rb;
        this.brain = brain;
        this.carData = (CarData) carData;

        facingOffsetDegrees = GetFacingOffsetDegrees(spriteFacing);

        if ((rb.constraints & RigidbodyConstraints2D.FreezeRotation) != 0)
        {
            Debug.LogWarning($"{gameObject.name}: Rigidbody2D had Freeze Rotation Z enabled - this blocks car steering entirely. Auto-disabling it. Uncheck it in the Inspector to remove this warning.");
            rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void Initialize(EntityBrain brain, SpriteRenderer spriteRenderer, Rigidbody2D rb)
    {
        throw new System.NotImplementedException();
    }
    private static float GetFacingOffsetDegrees(SpriteFacing facing)
    {
        switch (facing)
        {
            case SpriteFacing.Up: return 0f;
            case SpriteFacing.Down: return 180f;
            case SpriteFacing.Right: return -90f;
            case SpriteFacing.Left: return 90f;
            default: return 0f;
        }
    }

    public void SetMovement(Vector2 desiredDirection, float speed)
    {
        bool wantsToMove = desiredDirection != Vector2.zero && speed > 0f;

        if (!wantsToMove)
        {
            CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, 0f, carData.BrakeDeceleration * Time.deltaTime);
            rb.velocity = Forward * CurrentSpeed;
            SetReversing(false);
            return;
        }

        // Steer: rotate the car toward the desired direction, limited by turnSpeed.
        float angleToTarget = GetStableSteeringAngle(desiredDirection);
        float absAngle = Mathf.Abs(angleToTarget);

        float step = carData.TurnSpeed * Time.deltaTime;
        float appliedTurn = Mathf.Clamp(angleToTarget, -step, step);
        rb.MoveRotation(rb.rotation + appliedTurn);
        if (Mathf.Abs(appliedTurn) > 0.001f) lastTurnSign = Mathf.Sign(appliedTurn);
        Vector2 newForward = GetForwardFromRotation(rb.rotation + appliedTurn);

        bool shouldReverse = absAngle > carData.ReverseAngleThreshold;

        if (shouldReverse)
        {
            CurrentSpeed = speed * carData.ReverseSpeedMultiplier;
            rb.velocity = -newForward * CurrentSpeed;
        }
        else
        {
            float turnRatio = absAngle / 180f;
            float speedFactor = Mathf.Lerp(1f, 1f - carData.TurnSpeedPenalty, turnRatio);

            CurrentSpeed = speed * speedFactor;
            rb.velocity = newForward * CurrentSpeed;
        }

        SetReversing(shouldReverse);
    }

    private void SetReversing(bool reversing)
    {
        IsReversing = reversing;

        if (brain != null && brain.Animator != null && !string.IsNullOrEmpty(reversingBool))
        {
            brain.Animator.SetBool(reversingBool, reversing);
        }
    }

    public void FaceDirection(Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        float angleToTarget = GetStableSteeringAngle(direction);
        float step = carData.TurnSpeed * Time.deltaTime;
        float appliedTurn = Mathf.Clamp(angleToTarget, -step, step);
        rb.MoveRotation(rb.rotation + appliedTurn);
        if (Mathf.Abs(appliedTurn) > 0.001f) lastTurnSign = Mathf.Sign(appliedTurn);
    }
    private float GetStableSteeringAngle(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(Forward, direction);

        if (Mathf.Abs(angle) > 179f)
        {
            return lastTurnSign * Mathf.Abs(angle);
        }

        return angle;
    }
    public bool IsAlignedWithDirection(Vector2 direction)
    {
        if (direction == Vector2.zero) return false;
        return Vector2.Angle(Forward, direction) <= forwardFireConeAngle * 0.5f;
    }

    private Vector2 GetForwardFromRotation(float zRotationDegrees)
    {
        float angle = (zRotationDegrees + facingOffsetDegrees) * Mathf.Deg2Rad;
        return new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
    }

}