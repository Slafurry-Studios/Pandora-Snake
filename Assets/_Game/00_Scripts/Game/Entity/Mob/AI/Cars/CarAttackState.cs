using Game.Gameplay;
using Slafurry.System.Audio;
using UnityEngine;

namespace Game.AI
{
    /// <summary>
    /// Optional hostile car attack. Same plug-and-play Bullet/BulletManager pipeline as
    /// Humanoid's AttackState, but a car can't instantly snap-aim: it can only fire
    /// straight along its own nose. This state keeps steering the car toward the target
    /// (so it drives itself into alignment like a real vehicle) and only fires once
    /// CarMovement reports the target is inside the forward-fire cone.
    /// Leave this state off a car's stateList entirely for a non-hostile car.
    /// </summary>
    public class CarAttackState : EntityState
    {
        [Header("Engagement Settings")]
        public float attackRadius = 15f;
        public float pursuitSpeedMultiplier = 1.2f;
        public float fireRate = 0.6f;

        [Header("Animation")]
        [Tooltip("Animator bool set to true for the whole time this state is active. Combined with CarMovement's IsReversing, this drives the animator between Foward_shoot and Backward_shoot.")]
        [SerializeField] private string shootingBool = "IsShooting";
        [SerializeField] private string attackSound;

        [Header("Bullet Settings")]
        [Tooltip("Drag the Bullet prefab here (it must have the Bullet script attached)")]
        public Bullet bulletPrefab;
        public Transform firePoint;
        public float bulletDamage = 1f;
        public float bulletSpeed = 12f;
        public float bulletMaxDistance = 20f;
        public float bulletHitRadius = 0.2f;
        [Tooltip("What should this bullet hit? (Set to 'Player')")]
        public LayerMask targetMask;

        [Header("Line of Sight (LoS)")]
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
            carMovement = brain.Movement as CarMovement;
            if (carMovement == null)
            {
                Debug.LogWarning($"{gameObject.name}: CarAttackState works best with a CarMovement component (needed to know the forward-fire cone). Falling back to raw direction-to-target.");
            }

            SetShootingAnim(brain, true);
        }

        public override void UpdateState(EntityBrain brain)
        {
            var player = brain.Target.GetComponent<Game.Player.PlayerMovement>();
            float playerSpeed = (player != null) ? player.CurrentSpeed : 5f;

            Vector2 directionToTarget = (brain.Target.position - transform.position).normalized;

            // Keep steering toward the target so the car lines its own nose up for a shot,
            // instead of snap-aiming like Humanoid's AttackState does.
            brain.Movement.SetMovement(directionToTarget, playerSpeed * pursuitSpeedMultiplier);

            bool isLinedUp = carMovement != null
                ? carMovement.IsAlignedWithDirection(directionToTarget)
                : true; // no CarMovement to check against - fall back to always allowed

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
            if (brain.aiAnimation != null && !string.IsNullOrEmpty(shootingBool))
            {
                brain.aiAnimation.SetBool(shootingBool, isShooting);
            }
        }

        private void Fire(EntityBrain brain, Vector2 fallbackDirection)
        {
            if (bulletPrefab == null || firePoint == null)
            {
                Debug.LogWarning($"{gameObject.name} missing Bullet Prefab or Fire Point!");
                lastFireTime = Time.time;
                return;
            }

            // Bullet always leaves along the car's actual nose - it can only shoot forward.
            Vector2 fireDirection = carMovement != null ? carMovement.Forward : fallbackDirection;

            BulletManager.Instance.FireBullet(
                bulletPrefab,
                firePoint.position,
                fireDirection,
                bulletDamage,
                bulletSpeed,
                bulletMaxDistance,
                targetMask,
                bulletHitRadius,
                false,
                false
            );

            Audio.PlaySFX2D("Car", attackSound);

            lastFireTime = Time.time;
        }
    }
}