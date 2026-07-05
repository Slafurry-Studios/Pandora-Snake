using UnityEngine;

namespace Game.AI
{
    public class ChaseState : EntityState
    {
        [Header("Chase Settings")]
        public float chaseRadius = 15f;

        public float speedMultiplier = 0.8f;

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