using UnityEngine;
using Game.Player;

namespace Game.Entities.Boss
{
    public class BossChaseState : EntityState
    {
        [Header("Chase Configuration")]
        [Tooltip("Maximum distance from target (in units) at which the boss detects and starts chasing the player. Default: 25 units.")]
        public float detectionRadius = 25f;

        [Tooltip("Preferred standoff distance (in units) to maintain between the boss and the target. Default: 8 units.")]
        public float maintainDistance = 8f;

        [Tooltip("Speed multiplier relative to the player's current speed. For example, 1.2 moves 20% faster than the player. Default: 1.2x.")]
        public float movementSpeedMultiplier = 1.2f;

        [Header("Rotation")]
        [Tooltip("Degrees per second the boss rotates to face its movement direction. Default: 180.")]
        public float rotationSpeed = 180f;

        [Tooltip("Angle offset (degrees) to correct for the sprite's default facing direction. Default: 90 (sprite faces down at rotation 0).")]
        public float spriteFacingOffset = 90f;

        public override bool CheckConditions(EntityBrain brain)
        {
            if (brain.Target == null) return false;

            BossHealth health = brain.GetComponent<BossHealth>();
            if (health != null && health.IsDead) return false;

            float distance = Vector2.Distance(transform.position, brain.Target.position);
            return distance <= detectionRadius;
        }

        public override void EnterState(EntityBrain brain)
        {
        }

        public override void UpdateState(EntityBrain brain)
        {
            if (brain.Target == null || brain.EntityMovement == null) return;

            BossHealth health = brain.GetComponent<BossHealth>();
            if (health != null && health.IsDead)
            {
                brain.EntityMovement.SetMovement(Vector2.zero, 0f);
                return;
            }

            PlayerMovement player = brain.Target.GetComponent<PlayerMovement>();
            float playerSpeed = (player != null) ? player.CurrentSpeed : 5f;
            float chaseSpeed = playerSpeed * movementSpeedMultiplier;

            float distance = Vector2.Distance(transform.position, brain.Target.position);
            Vector2 directionToTarget = (brain.Target.position - transform.position).normalized;

            if (distance > maintainDistance + 0.5f)
            {
                brain.EntityMovement.SetMovement(directionToTarget, chaseSpeed);
                RotateTowards(directionToTarget);
            }
            else if (distance < maintainDistance - 0.5f)
            {
                brain.EntityMovement.SetMovement(-directionToTarget, chaseSpeed);
                RotateTowards(-directionToTarget);
            }
            else
            {
                brain.EntityMovement.SetMovement(Vector2.zero, 0f);
                RotateTowards(directionToTarget);
            }
        }

        private void RotateTowards(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.0001f) return;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + spriteFacingOffset;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public override void ExitState(EntityBrain brain)
        {
            if (brain.EntityMovement != null)
            {
                brain.EntityMovement.SetMovement(Vector2.zero, 0f);
            }
        }
    }
}