using UnityEngine;

namespace Game.AI
{
    /// <summary>
    /// Non-hostile flee behavior: the car steers away from the target once it gets too
    /// close, and keeps fleeing (with hysteresis) until it feels safe again. Useful for
    /// civilian cars that panic when a hostile entity or the player gets near.
    /// </summary>
    public class CarPanicState : EntityState
    {
        [Header("Panic Thresholds")]
        [Tooltip("How close the target must be to trigger panic.")]
        public float panicRadius = 6f;

        [Tooltip("How far away the target must be before the car feels safe again.")]
        public float safeRadius = 10f;

        [Header("Movement")]
        public float speedMultiplier = 1.5f;

        private bool isPanicking = false;

        public override bool CheckConditions(EntityBrain brain)
        {
            if (brain.Target == null) return false;

            float distance = Vector2.Distance(transform.position, brain.Target.position);

            if (isPanicking)
            {
                return distance <= safeRadius;
            }

            return distance <= panicRadius;
        }

        public override void EnterState(EntityBrain brain)
        {
            isPanicking = true;
        }

        public override void UpdateState(EntityBrain brain)
        {
            var player = brain.Target.GetComponent<Game.Player.PlayerMovement>();
            float playerSpeed = (player != null) ? player.CurrentSpeed : 5f;
            float fleeSpeed = playerSpeed * speedMultiplier;

            Vector2 fleeDirection = ((Vector2)transform.position - (Vector2)brain.Target.position).normalized;
            brain.Movement.SetMovement(fleeDirection, fleeSpeed);
        }

        public override void ExitState(EntityBrain brain)
        {
            isPanicking = false;
            brain.Movement.SetMovement(Vector2.zero, 0f);
        }
    }
}