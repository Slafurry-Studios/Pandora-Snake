using Game.Entities;
using UnityEngine;

namespace Game.Entities
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
            float chaseSpeed = entityData.MoveSpeed * speedMultiplier;

            Vector2 moveDirection = (brain.Target.position - transform.position).normalized;

            brain.EntityMovement.SetMovement(moveDirection, chaseSpeed);
        }

        public override void ExitState(EntityBrain brain)
        {
            brain.EntityMovement.SetMovement(Vector2.zero, 0f);
        }
    }
}