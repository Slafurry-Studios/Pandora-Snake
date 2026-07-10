using UnityEngine;

namespace Game.Entities
{
    public class AttackState : EntityState
    {
        [Header("Combat Settings")]
        public float attackRadius = 10f;
        public float fireRate = 0.5f;

        [Header("Animation")]
        [SerializeField] private string attackAnim;

        [Header("Line of Sight (LoS)")]
        [SerializeField] private bool requiresLineOfSight = true;
        [SerializeField] LayerMask obstacleLayer;

        private float lastFireTime;

        public override bool CheckConditions(EntityBrain brain)
        {
            if (brain.Target == null) return false;

            float distance = Vector2.Distance(transform.position, brain.Target.position);

            if (distance > attackRadius) return false;

            if (requiresLineOfSight)
            {
                Vector2 directionToTarget = (brain.Target.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, distance, obstacleLayer);

                if (hit.collider != null) return false;
            }

            return true;
        }

        public override void EnterState(EntityBrain brain)
        {
            brain.EntityMovement.SetMovement(Vector2.zero, 0f);
        }

        public override void UpdateState(EntityBrain brain)
        {
            brain.EntityMovement.SetMovement(Vector2.zero, 0f);

            Vector2 aimDirection = (brain.Target.position - transform.position).normalized;
            brain.EntityMovement.FaceDirection(aimDirection);

            if (Time.time >= lastFireTime + fireRate)
            {
                brain.EntityShoot.Shoot(aimDirection);
            }
            else
            {
                Debug.LogWarning(
                    $"{gameObject.name} missing Bullet Prefab or Fire Point!"
                );
            }


            lastFireTime = Time.time;
        }

        public override void ExitState(EntityBrain brain)
        {
            if (brain.Animator != null && !string.IsNullOrEmpty(attackAnim))
            {
                brain.Animator.ResetTrigger(attackAnim);
            }
        }
    }
}