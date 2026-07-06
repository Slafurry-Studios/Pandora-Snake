using Game.AI;
using UnityEngine;

/// <summary>
/// Vehicle movement handler for car-type entities. Unlike NPCMovement (free 2D velocity),
/// a car steers toward its desired direction and drives forward along its own nose -
/// it can't strafe or snap-turn. This also means a car can only ever fire "forward",
/// which CarAttackState relies on via IsAlignedWithDirection().
///
/// Different car sprites may be drawn facing different ways (some artists draw the nose
/// pointing up, some down). Set "Sprite Facing" in the Inspector to match this particular
/// sprite - the script handles the rest, no need to nest/rotate child objects.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class CarMovement : MonoBehaviour, IEntityMovement
{
    public enum SpriteFacing { Up, Down, Right, Left }

    [Header("Sprite Orientation")]
    [Tooltip("Which way this car's sprite is drawn facing at rotation 0. Must match the art - different artists/sprites may face different ways.")]
    public SpriteFacing spriteFacing = SpriteFacing.Up;

    [Header("Steering Settings")]
    [Tooltip("How fast the car can turn, in degrees per second.")]
    public float turnSpeed = 120f;

    [Tooltip("How much max speed is lost while turning sharply. 0 = no penalty, 1 = near-stop while turning hard.")]
    [Range(0f, 1f)] public float turnSpeedPenalty = 0.5f;

    [Tooltip("How quickly the car slows down when told to stop (units/sec^2).")]
    public float brakeDeceleration = 15f;

    [Header("Forward-Only Firing")]
    [Tooltip("Total cone angle (degrees) in front of the car considered 'lined up' for a shot.")]
    public float forwardFireConeAngle = 20f;

    [Header("Reverse Driving")]
    [Tooltip("If the desired direction is more than this many degrees away from the car's nose, the car reverses instead of doing a slow full turn (like a real car backing up).")]
    public float reverseAngleThreshold = 120f;
    [Tooltip("Reverse speed as a fraction of the requested speed.")]
    [Range(0f, 1f)] public float reverseSpeedMultiplier = 0.6f;

    [Header("Animation")]
    [Tooltip("Animator bool set to true while the car is driving in reverse. Matches Animator states: Foward / Foward_shoot (false), Backward / Backward_shoot (true).")]
    [SerializeField] private string reversingBool = "IsReversing";

    private Rigidbody2D rb;
    private EntityBrain brain;
    private float facingOffsetDegrees;

    /// <summary>Current forward speed the car is actually driving at (after turn penalty/braking). Negative while reversing.</summary>
    public float CurrentSpeed { get; private set; }

    /// <summary>True while the car is currently driving in reverse.</summary>
    public bool IsReversing { get; private set; }

    /// <summary>The car's current forward direction (its nose), accounting for how the sprite is drawn.</summary>
    public Vector2 Forward => Quaternion.Euler(0f, 0f, facingOffsetDegrees) * transform.up;

    /// <summary>Which way the car turned last (+1 = counter-clockwise, -1 = clockwise).
    /// Used to break ties when the desired direction is ~180 degrees away, where
    /// floating-point noise can otherwise make it flip-flop direction every frame.</summary>
    private float lastTurnSign = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        brain = GetComponent<EntityBrain>();
        facingOffsetDegrees = GetFacingOffsetDegrees(spriteFacing);

        // Steering relies on rb.MoveRotation actually rotating the body. If "Freeze
        // Rotation Z" is checked on the Rigidbody2D (common on prefabs duplicated from
        // Humanoid, which never rotates), the car will silently always drive toward
        // world-up (its default facing) no matter what direction it's told to go.
        if ((rb.constraints & RigidbodyConstraints2D.FreezeRotation) != 0)
        {
            Debug.LogWarning($"{gameObject.name}: Rigidbody2D had Freeze Rotation Z enabled - this blocks car steering entirely. Auto-disabling it. Uncheck it in the Inspector to remove this warning.");
            rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
        }
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
            CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, 0f, brakeDeceleration * Time.deltaTime);
            rb.velocity = Forward * CurrentSpeed;
            SetReversing(false);
            return;
        }

        // Steer: rotate the car toward the desired direction, limited by turnSpeed.
        float angleToTarget = GetStableSteeringAngle(desiredDirection);
        float absAngle = Mathf.Abs(angleToTarget);

        float step = turnSpeed * Time.deltaTime;
        float appliedTurn = Mathf.Clamp(angleToTarget, -step, step);
        rb.MoveRotation(rb.rotation + appliedTurn);
        if (Mathf.Abs(appliedTurn) > 0.001f) lastTurnSign = Mathf.Sign(appliedTurn);

        // If the target is well behind the car, reverse toward it instead of doing a
        // slow full turn - a real car would just back up rather than loop all the way around.
        bool shouldReverse = absAngle > reverseAngleThreshold;

        if (shouldReverse)
        {
            CurrentSpeed = speed * reverseSpeedMultiplier;
            rb.velocity = -Forward * CurrentSpeed;
        }
        else
        {
            // Drive forward along the car's own nose, not straight at the target -
            // the car curves into the direction over time instead of teleport-facing it.
            float turnRatio = absAngle / 180f; // 0 = dead ahead, 1 = fully behind
            float speedFactor = Mathf.Lerp(1f, 1f - turnSpeedPenalty, turnRatio);

            CurrentSpeed = speed * speedFactor;
            rb.velocity = Forward * CurrentSpeed;
        }

        SetReversing(shouldReverse);
    }

    private void SetReversing(bool reversing)
    {
        IsReversing = reversing;

        if (brain != null && brain.aiAnimation != null && !string.IsNullOrEmpty(reversingBool))
        {
            brain.aiAnimation.SetBool(reversingBool, reversing);
        }
    }

    public void FaceDirection(Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        float angleToTarget = GetStableSteeringAngle(direction);
        float step = turnSpeed * Time.deltaTime;
        float appliedTurn = Mathf.Clamp(angleToTarget, -step, step);
        rb.MoveRotation(rb.rotation + appliedTurn);
        if (Mathf.Abs(appliedTurn) > 0.001f) lastTurnSign = Mathf.Sign(appliedTurn);
    }

    /// <summary>
    /// Same as Vector2.SignedAngle(transform.up, direction), except when the direction
    /// is (near) directly behind the car (~180 degrees), where it's numerically ambiguous
    /// which way is "shorter". In that case it keeps turning the same way it turned last,
    /// instead of flip-flopping between +180/-180 every frame.
    /// </summary>
    private float GetStableSteeringAngle(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(Forward, direction);

        if (Mathf.Abs(angle) > 179f)
        {
            return lastTurnSign * Mathf.Abs(angle);
        }

        return angle;
    }

    /// <summary>
    /// True if 'direction' currently falls inside the car's forward-fire cone -
    /// i.e. the car is lined up enough to actually take the shot.
    /// </summary>
    public bool IsAlignedWithDirection(Vector2 direction)
    {
        if (direction == Vector2.zero) return false;
        return Vector2.Angle(Forward, direction) <= forwardFireConeAngle * 0.5f;
    }
}