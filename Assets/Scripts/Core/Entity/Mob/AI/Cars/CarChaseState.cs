using UnityEngine;

namespace Game.AI
{
    /// <summary>
    /// Hostile car chasing the target. The car continuously steers toward the target's
    /// current position - CarMovement turns it in gradually, so the car naturally
    /// curves into the pursuit like a real vehicle instead of snapping to face it.
    /// </summary>
    public class CarChaseState : EntityState
    {
        [Header("Chase Settings")]
        public float chaseRadius = 20f;
        public float speedMultiplier = 1f;

        public override bool CheckConditions(EntityBrain brain)
        {
            if (brain.Target == null) return false;

            return Vector2.Distance(transform.position, brain.Target.position) <= chaseRadius;
        }

        public override void EnterState(EntityBrain brain) { }

        public override void UpdateState(EntityBrain brain)
        {
            var player = brain.Target.GetComponent<Game.Player.PlayerMovement>();
            float playerSpeed = (player != null) ? player.CurrentSpeed : 5f;
            float chaseSpeed = playerSpeed * speedMultiplier;

            Vector2 moveDirection = (brain.Target.position - transform.position).normalized;
            brain.Movement.SetMovement(moveDirection, chaseSpeed);
        }

        public override void ExitState(EntityBrain brain)
        {
            brain.Movement.SetMovement(Vector2.zero, 0f);
        }
    }
}
