using UnityEngine;
using UnityEngine.InputSystem;
using Game.Gameplay;

namespace Game.Player
{
    [RequireComponent(typeof(PlayerAim))]
    public class PlayerShoot : MonoBehaviour
    {
        [Header("Shooting Settings")]
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float bulletNormalRadius = 0.2f;
        [SerializeField] private Bullet bulletBiggerDakkaPrefab;
        [SerializeField] private float bulletBiggerDakkaRadius = 0.5f;
        [SerializeField] private Bullet bulletAverageBulletEnjoyerPrefab;
        [SerializeField] private float bulletAverageBulletEnjoyerPrefabRadius = 1f;
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private float bulletDamage = 10f;
        [SerializeField] private float bulletSpeed = 20f;
        [SerializeField] private float maxShootDistance = 10f;
        [SerializeField] private LayerMask targetMask;

        [Header("Multi-Shot Settings")]
        [Tooltip("Jarak antar sudul tembakan (kelipatan 5).")]
        [SerializeField] private int bulletCount = 1;
        [SerializeField] private float angleSpacing = 5f;

        private PlayerAim playerAim;
        private float nextFireTime;
        public bool isExplosive = false;
        public bool isRichochet = false;

        private void Awake()
        {
            playerAim = GetComponent<PlayerAim>();
        }

        private void Update()
        {
            if (bulletPrefab == null || BulletManager.Instance == null)
                return;

            if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
        }

        private void Shoot()
        {
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
            BulletManager.Instance.FireBullet(
                bulletPrefab,
                playerAim.AimIndicatorPosition,
                dir,
                bulletDamage,
                bulletSpeed,
                maxShootDistance,
                targetMask,
                bulletNormalRadius,
                isExplosive,
                isRichochet);
            SoundManager.Instance.PlaySound2D("Shotgun_Punchy");
        }

        public void BiggerDakka()
        {
            bulletPrefab = bulletBiggerDakkaPrefab;
            bulletNormalRadius = bulletBiggerDakkaRadius;
        }

        public void MoreDakka()
        {
            bulletCount += 1;
        }

        public void DakkaEverywhere()
        {
            bulletCount += 2;
        }

        public void MoreEspresso()
        {
            fireRate *= 0.85f;
        }

        public void FrameRateKiller()
        {
            fireRate *= 0.7f;
        }
        public void AverageBulletEnjoyer()
        {
            bulletPrefab = bulletAverageBulletEnjoyerPrefab;
            bulletNormalRadius = bulletAverageBulletEnjoyerPrefabRadius;
        }

        public void ExplosiveAmmo()
        {
            isExplosive = true;
        }

        public void RichochetBullet()
        {
            isRichochet = true;
        }

        public void ApocalypseStream()
        {
            bulletCount += 5;
            fireRate *= 0.5f;
        }
    }
}