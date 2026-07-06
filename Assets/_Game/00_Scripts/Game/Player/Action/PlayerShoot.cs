using UnityEngine;
using UnityEngine.InputSystem;
using Game.Gameplay;
using Slafurry.System.Audio;
using Game.Managerd;

namespace Game.Player
{
    [RequireComponent(typeof(PlayerAim))]
    public class PlayerShoot : MonoBehaviour
    {
        private Bullet bulletPrefab;
        private float bulletBaseRadius = 0.2f;
        private float bulletScale = 1f;

        private float fireRate = 0.5f;
        private float bulletDamage = 10f;
        private float bulletSpeed = 20f;
        private float maxShootDistance = 10f;
        private LayerMask targetMask;
        private int bulletCount = 1;
        private float angleSpacing = 5f;

        private string shootSound;


        private PlayerAim playerAim;
        private float nextFireTime;
        private bool isExplosive = false;
        private bool isRichochet = false;

        public void Initialize()
        {
            Player player = GetComponentInParent<Player>();
            PlayerCombatData playerCombatData = player.PlayerData.CombatData;

            bulletPrefab = playerCombatData.BulletPrefab;
            bulletBaseRadius = playerCombatData.BulletRadius;
            bulletScale = playerCombatData.BulletScale;
            
            bulletDamage = playerCombatData.BulletDamage;
            bulletSpeed = playerCombatData.BulletSpeed;

            fireRate = playerCombatData.FireRate;
            maxShootDistance = playerCombatData.ShootDistance;

            targetMask = playerCombatData.TargetMask;

            bulletCount = playerCombatData.BulletCount;
            angleSpacing = playerCombatData.AngleSpacing;

            shootSound = player.PlayerData.ShootSFX;

            playerAim = GetComponent<PlayerAim>();
        }

        private void Update()
        {
            if (bulletPrefab == null || GameManager.Bullet == null)
                return;

            if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
        }

        private void Shoot()
        {
            Audio.PlaySFX2D("Player", shootSound);

            Vector3 currentAimDirection = playerAim.CurrentAimDirection;
            if (currentAimDirection == Vector3.zero)
                return;

            foreach (float angle in GetShotAngles())
            {
                Vector3 dir = Quaternion.Euler(0, 0, angle) * currentAimDirection;
                SpawnBullet(dir);
            }
        }

        private float[] GetShotAngles()
        {
            float[] angles = new float[bulletCount];
            float startAngle = -(bulletCount - 1) * angleSpacing * 0.5f;

            for (int i = 0; i < bulletCount; i++)
            {
                angles[i] = startAngle + (i * angleSpacing);
            }

            return angles;
        }

        private void SpawnBullet(Vector3 dir)
        {
            BulletFireData bulletFireData = new()
            {
                prefab = bulletPrefab,
                startPos = playerAim.AimIndicatorPosition,
                direction = dir,
                damage = bulletDamage,
                speed = bulletSpeed,
                maxDistance = maxShootDistance,
                targetMask = targetMask,
                hitRadius = bulletBaseRadius * bulletScale,
                isExplosive = isExplosive,
                isRichochet = isRichochet,
                scale = bulletScale
            };
            GameManager.Bullet.FireBullet(bulletFireData);
        }

        public void BiggerDakka() => bulletScale = 2.5f;
        public void AverageBulletEnjoyer() => bulletScale = 5f;

        public void MoreDakka() => bulletCount += 1;
        public void DakkaEverywhere() => bulletCount += 2;
        public void MoreEspresso() => fireRate *= 0.85f;
        public void FrameRateKiller() => fireRate *= 0.7f;

        public void ExplosiveAmmo()
        {
            isExplosive = true;
            bulletDamage *= 5;
        }

        public void RichochetBullet() => isRichochet = true;

        public void ApocalypseStream()
        {
            bulletCount += 5;
            fireRate *= 0.5f;
        }
    }
}