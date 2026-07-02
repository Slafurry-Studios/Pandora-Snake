using Game.Gameplay;
using UnityEngine;

namespace Game.AI
{
    public class AttackState : EntityState
    {
        [Header("Combat Settings")]
        public float attackRadius = 10f;
        public float fireRate = 0.5f;

        [Header("Animation")]
        [SerializeField] private string attackAnim;
        [SerializeField] private string attackSound;

        [Header("Bullet Settings")]
        [Tooltip("Drag the Bullet prefab here (it must have the Bullet script attached)")]
        public Bullet bulletPrefab;
        public Transform firePoint;
        public float bulletDamage = 1f;
        public float bulletSpeed = 10f;
        public float bulletMaxDistance = 20f;
        public float bulletHitRadius = 0.2f;
        [Tooltip("What should this bullet hit? (Set to 'Player')")]
        public LayerMask targetMask;

        [Header("Line of Sight (LoS)")]
        public bool requiresLineOfSight = true;
        public LayerMask obstacleLayer;

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
            brain.Movement.SetMovement(Vector2.zero, 0f);
        }

        public override void UpdateState(EntityBrain brain)
        {
            brain.Movement.SetMovement(Vector2.zero, 0f);

            Vector2 aimDirection = (brain.Target.position - transform.position).normalized;
            brain.Movement.FaceDirection(aimDirection);

            if (Time.time >= lastFireTime + fireRate)
            {
                if (bulletPrefab != null && firePoint != null)
                {
                    BulletManager.Instance.FireBullet(
                        bulletPrefab,
                        firePoint.position,
                        aimDirection,
                        bulletDamage,
                        bulletSpeed,
                        bulletMaxDistance,
                        targetMask,
                        bulletHitRadius,
                        false,
                        false
                    );

                    SoundManager.Instance.PlaySound2D(attackSound);


                    // Trigger attack hanya saat menembak
                    if (brain.aiAnimation != null && !string.IsNullOrEmpty(attackAnim))
                    {
                        brain.aiAnimation.ResetTrigger(attackAnim);
                        brain.aiAnimation.SetTrigger(attackAnim);
                    }
                }
                else
                {
                    Debug.LogWarning(
                        $"{gameObject.name} missing Bullet Prefab or Fire Point!"
                    );
                }


                lastFireTime = Time.time;
            }
        }
        public override void ExitState(EntityBrain brain)
        {
            if (brain.aiAnimation != null && !string.IsNullOrEmpty(attackAnim))
            {
                brain.aiAnimation.ResetTrigger(attackAnim);
            }
        }
    }
}