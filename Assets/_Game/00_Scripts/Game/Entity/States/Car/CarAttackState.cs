using Game.Gameplay;
using UnityEngine;

namespace Game.Entities
{
    public class CarAttackState : EntityState
    {
        [Header("Engagement Settings")]
        public float attackRadius = 15f;
        public float pursuitSpeedMultiplier = 1.2f;
        public float fireRate = 0.6f;

        [Header("Animation")]
        [Tooltip("Animator bool set to true for the whole time this state is active. Combined with CarMovement's IsReversing, this drives the animator between Foward_shoot and Backward_shoot.")]
        [SerializeField] private string shootingBool = "IsShooting";

        [Header("Line of Sight (LoS)")]
        [SerializeField] private Transform firePoint;
        public bool requiresLineOfSight = true;
        public LayerMask obstacleLayer;

        private float lastFireTime;
        private CarMovement carMovement;

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
            carMovement = brain.EntityMovement as CarMovement;
            if (carMovement == null)
            {
                Debug.LogWarning($"{gameObject.name}: CarAttackState works best with a CarMovement component (needed to know the forward-fire cone). Falling back to raw direction-to-target.");
            }

            SetShootingAnim(brain, true);
        }

        public override void UpdateState(EntityBrain brain)
        {
            Vector2 directionToTarget = (brain.Target.position - transform.position).normalized;

            brain.EntityMovement.SetMovement(directionToTarget, entityData.MoveSpeed);

            bool isLinedUp = carMovement != null
                ? carMovement.IsAlignedWithDirection(directionToTarget)
                : true;
            if (isLinedUp && Time.time >= lastFireTime + fireRate)
            {
                Fire(brain, directionToTarget);
            }
        }

        public override void ExitState(EntityBrain brain)
        {
            SetShootingAnim(brain, false);
        }

        private void SetShootingAnim(EntityBrain brain, bool isShooting)
        {
            if (brain.Animator != null && !string.IsNullOrEmpty(shootingBool))
            {
                brain.Animator.SetBool(shootingBool, isShooting);
            }
        }

        private void Fire(EntityBrain brain, Vector2 fallbackDirection)
        {
            if (firePoint == null)
            {
                Debug.LogWarning($"{gameObject.name} missing Bullet Prefab or Fire Point!");
                lastFireTime = Time.time;
                return;
            }

            Vector2 aimDirection = carMovement != null ? carMovement.Forward : fallbackDirection;

            brain.EntityShoot.Shoot(aimDirection);

            lastFireTime = Time.time;
        }
    }
}