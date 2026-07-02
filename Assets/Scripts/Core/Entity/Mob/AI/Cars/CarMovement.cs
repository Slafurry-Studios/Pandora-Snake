using Game.AI;
using UnityEngine;

/// <summary>
/// Vehicle movement handler for car-type entities. Unlike NPCMovement (free 2D velocity),
/// a car steers toward its desired direction and drives forward along its own nose -
/// it can't strafe or snap-turn. This also means a car can only ever fire "forward",
/// which CarAttackState relies on via IsAlignedWithDirection().
///
/// Assumes the car sprite's nose points "up" (transform.up) at rotation 0, matching the
/// usual top-down car sprite convention. If your art faces a different way, offset the
/// sprite inside a child object rather than changing this script.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class CarMovement : MonoBehaviour, IEntityMovement
{
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

    [Header("Animation")]
    [SerializeField] private string drivingBool = "Driving";

    private Rigidbody2D rb;
    private EntityBrain brain;

    /// <summary>Current forward speed the car is actually driving at (after turn penalty/braking).</summary>
    public float CurrentSpeed { get; private set; }

    /// <summary>The car's current forward direction (its nose).</summary>
    public Vector2 Forward => transform.up;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        brain = GetComponent<EntityBrain>();
    }

    public void SetMovement(Vector2 desiredDirection, float speed)
    {
        bool wantsToMove = desiredDirection != Vector2.zero && speed > 0f;

        if (brain != null && brain.aiAnimation != null && !string.IsNullOrEmpty(drivingBool))
        {
            brain.aiAnimation.SetBool(drivingBool, wantsToMove);
        }

        if (!wantsToMove)
        {
            CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, 0f, brakeDeceleration * Time.deltaTime);
            rb.velocity = (Vector2)transform.up * CurrentSpeed;
            return;
        }

        // Steer: rotate the car toward the desired direction, limited by turnSpeed.
        float angleToTarget = Vector2.SignedAngle(transform.up, desiredDirection);
        float step = turnSpeed * Time.deltaTime;
        float appliedTurn = Mathf.Clamp(angleToTarget, -step, step);
        rb.MoveRotation(rb.rotation + appliedTurn);

        // Drive forward along the car's own nose, not straight at the target -
        // the car curves into the direction over time instead of teleport-facing it.
        float turnRatio = Mathf.Abs(angleToTarget) / 180f; // 0 = dead ahead, 1 = fully behind
        float speedFactor = Mathf.Lerp(1f, 1f - turnSpeedPenalty, turnRatio);

        CurrentSpeed = speed * speedFactor;
        rb.velocity = (Vector2)transform.up * CurrentSpeed;
    }

    public void FaceDirection(Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        float angleToTarget = Vector2.SignedAngle(transform.up, direction);
        float step = turnSpeed * Time.deltaTime;
        float appliedTurn = Mathf.Clamp(angleToTarget, -step, step);
        rb.MoveRotation(rb.rotation + appliedTurn);
    }

    /// <summary>
    /// True if 'direction' currently falls inside the car's forward-fire cone -
    /// i.e. the car is lined up enough to actually take the shot.
    /// </summary>
    public bool IsAlignedWithDirection(Vector2 direction)
    {
        if (direction == Vector2.zero) return false;
        return Vector2.Angle(transform.up, direction) <= forwardFireConeAngle * 0.5f;
    }
}
