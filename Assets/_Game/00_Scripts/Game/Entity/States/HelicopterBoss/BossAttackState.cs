using UnityEngine;
using Game.Gameplay;
using Game.Managerd;

namespace Game.Entities.Boss
{
    [System.Serializable]
    public class BossWeaponProfile
    {
        [Header("Weapon Identification")]
        [Tooltip("Name of the weapon or attack pattern.")]
        public string weaponName = "New Weapon";
        [Tooltip("IMPORTANT: Drag your Bullet prefab (e.g. BossBullet or standard Bullet) here! If null, the weapon cannot shoot!")]
        public Bullet bulletPrefab;

        [Header("Firing Cycle Setup")]
        [Tooltip("Number of shots fired in rapid succession per burst cycle.")]
        public int shotsPerCycle = 1;
        [Tooltip("Total duration (in seconds) of one complete burst cycle including cooldown interval before the next burst starts.")]
        public float cycleDuration = 1f;
        [Tooltip("Time delay (in seconds) between individual shots inside a single burst.")]
        public float intervalBetweenShots = 0.15f;

        [Header("Projectile Properties")]
        [Tooltip("Damage dealt by each individual projectile upon impact.")]
        public float damage = 1f;
        [Tooltip("Speed (units per second) at which the projectile travels across the screen.")]
        public float speed = 10f;
        [Tooltip("Maximum travel distance (in units) before the projectile despawns.")]
        public float range = 30f;
        [Tooltip("Layer mask indicating which objects this projectile can hit. If set to Nothing (0), automatically defaults to hitting Player layers.")]
        public LayerMask targetMask;
        [Tooltip("Collision detection radius of the projectile.")]
        public float hitRadius = 0.5f;
    }

    public class BossAttackState : EntityState
    {
        [Header("Attack Range & Setup")]
        [Tooltip("Maximum distance from target (in units) required to enter attack mode. Default: 20 units.")]
        public float attackRadius = 20f;

        [Tooltip("Optional child Transform representing the exact muzzle/gun tip where projectiles spawn. If left unassigned, spawns slightly ahead of the boss center automatically.")]
        public Transform firePoint;

        [Header("Line of Sight")]
        [Tooltip("If checked, the boss will only attack when there are no solid obstacles between it and the target. Default: false.")]
        public bool requiresLineOfSight = false;
        [Tooltip("Layer mask representing solid obstacles that block line of sight (e.g., Buildings or Walls).")]
        public LayerMask obstacleLayer;

        [Header("Rotation")]
        [Tooltip("Degrees per second the boss rotates to face the target while attacking. Default: 180.")]
        public float rotationSpeed = 180f;

        [Tooltip("Angle offset (degrees) to correct for the sprite's default facing direction. Default: 90 (sprite faces down at rotation 0).")]
        public float spriteFacingOffset = 90f;

        [Header("Phase Thresholds (Weapon Switching)")]
        [Tooltip("HP Threshold X (0 to 1). Above X%, boss uses BURST BULLET exclusively. Default: 0.7 (70%).")]
        [Range(0f, 1f)]
        public float hpThresholdPhaseX = 0.7f;

        [Tooltip("HP Threshold Y (0 to 1). Below Y%, boss randomly switches between both weapons. Between Y% and X%, boss uses HEAVY MISSILE. Default: 0.35 (35%).")]
        [Range(0f, 1f)]
        public float hpThresholdPhaseY = 0.35f;

        [Header("Weapon Profiles")]
        [Tooltip("Weapon configuration profile for heavy missile attacks.")]
        public BossWeaponProfile heavyMissileProfile = new BossWeaponProfile
        {
            weaponName = "HEAVY MISSILE",
            shotsPerCycle = 2,
            cycleDuration = 2.75f,
            intervalBetweenShots = 0.25f,
            damage = 2f,
            speed = 20f,
            range = 30f,
            hitRadius = 0.75f
        };

        [Tooltip("Weapon configuration profile for burst bullet attacks.")]
        public BossWeaponProfile burstBulletProfile = new BossWeaponProfile
        {
            weaponName = "BURST BULLET",
            shotsPerCycle = 5,
            cycleDuration = 1.2f,
            intervalBetweenShots = 0.15f,
            damage = 1f,
            speed = 18f,
            range = 30f,
            hitRadius = 0.25f
        };

        private BossWeaponProfile activeProfile;
        private float nextCycleTime;
        private float nextShotTime;
        private int shotsRemainingInCycle;

        public BossWeaponProfile CurrentWeaponProfile => activeProfile;

        public override bool CheckConditions(EntityBrain brain)
        {
            if (brain.Target == null) return false;

            BossHealth health = brain.GetComponent<BossHealth>();
            if (health != null && health.IsDead) return false;

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
            if (activeProfile == null)
            {
                SelectNextWeapon(brain);
            }
        }

        public override void UpdateState(EntityBrain brain)
        {
            if (brain.Target == null || activeProfile == null) return;

            BossHealth health = brain.GetComponent<BossHealth>();
            if (health != null && health.IsDead)
            {
                if (brain.EntityMovement != null) brain.EntityMovement.SetMovement(Vector2.zero, 0f);
                return;
            }

            Vector2 aimDirection = (brain.Target.position - transform.position).normalized;

            RotateTowards(aimDirection);

            if (brain.EntityMovement != null)
            {
                Game.Player.PlayerMovement player = brain.Target.GetComponent<Game.Player.PlayerMovement>();
                float playerSpeed = (player != null) ? player.CurrentSpeed : 5f;
                float moveSpeed = playerSpeed * 1.2f;
                float targetDistance = 8f;

                BossChaseState chaseState = GetComponent<BossChaseState>();
                if (chaseState != null)
                {
                    moveSpeed = playerSpeed * chaseState.movementSpeedMultiplier;
                    targetDistance = chaseState.maintainDistance;
                }

                float currentDist = Vector2.Distance(transform.position, brain.Target.position);
                if (currentDist > targetDistance + 0.5f)
                {
                    brain.EntityMovement.SetMovement(aimDirection, moveSpeed);
                }
                else if (currentDist < targetDistance - 0.5f)
                {
                    brain.EntityMovement.SetMovement(-aimDirection, moveSpeed);
                }
                else
                {
                    brain.EntityMovement.SetMovement(Vector2.zero, 0f);
                }
            }

            if (shotsRemainingInCycle <= 0 && Time.time >= nextCycleTime)
            {
                shotsRemainingInCycle = activeProfile.shotsPerCycle;
                nextShotTime = Time.time;
            }

            if (shotsRemainingInCycle > 0 && Time.time >= nextShotTime)
            {
                FireProjectile(aimDirection);

                shotsRemainingInCycle--;
                nextShotTime = Time.time + activeProfile.intervalBetweenShots;

                if (shotsRemainingInCycle <= 0)
                {
                    nextCycleTime = Time.time + activeProfile.cycleDuration;
                    SelectNextWeapon(brain);
                }
            }
        }

        private void RotateTowards(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.0001f) return;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + spriteFacingOffset;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void FireProjectile(Vector2 direction)
        {
            if (activeProfile == null) return;
            if (activeProfile.bulletPrefab == null)
            {
                Debug.LogError($"[BossAttackState] CANNOT SHOOT! Weapon '{activeProfile.weaponName}' has NO Bullet Prefab assigned in the Inspector! Please assign a Bullet Prefab under Weapon Profiles -> Bullet Prefab on {gameObject.name}!");
                return;
            }
            if (GameManager.Bullet == null)
            {
                Debug.LogWarning("[BossAttackState] BulletManager.Instance is missing in the scene!");
                return;
            }

            Vector2 spawnPos = (firePoint != null) ? (Vector2)firePoint.position : (Vector2)transform.position + direction * 1.5f;

            LayerMask maskToUse = activeProfile.targetMask;
            if (maskToUse.value == 0)
            {
                int playerLayer = LayerMask.NameToLayer("Player");
                if (playerLayer != -1)
                {
                    maskToUse = 1 << playerLayer;
                }
                else
                {
                    maskToUse = ~0;
                }
            }


            BulletData bulletFireData = new BulletData
            {
                prefab = activeProfile.bulletPrefab,
                startPos = firePoint.position,
                direction = direction,
                damage = activeProfile.damage,
                speed = activeProfile.speed,
                targetMask = maskToUse,
                hitRadius = activeProfile.hitRadius,
                isExplosive = false,
                isRichochet = false,
                scale = 1f
            };

            GameManager.Bullet.FireBullet(bulletFireData);
        }

        protected virtual void SelectNextWeapon(EntityBrain brain)
        {
            if (heavyMissileProfile == null || burstBulletProfile == null)
            {
                activeProfile = heavyMissileProfile ?? burstBulletProfile;
                return;
            }

            BossHealth health = GetComponentInParent<BossHealth>();
            if (health != null && health.MaxHealth > 0)
            {
                float hpPct = health.CurrentHealth / health.MaxHealth;

                if (hpPct > hpThresholdPhaseX)
                {
                    activeProfile = burstBulletProfile;
                }
                else if (hpPct > hpThresholdPhaseY)
                {
                    activeProfile = heavyMissileProfile;
                }
                else
                {
                    activeProfile = (Random.value > 0.5f) ? heavyMissileProfile : burstBulletProfile;
                }
            }
            else
            {
                activeProfile = (Random.value > 0.5f) ? heavyMissileProfile : burstBulletProfile;
            }
        }

        public override void ExitState(EntityBrain brain)
        {
            shotsRemainingInCycle = 0;
        }
    }
}