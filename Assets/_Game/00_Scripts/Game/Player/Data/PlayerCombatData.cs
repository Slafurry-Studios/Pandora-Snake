using Game.Gameplay;
using UnityEngine;

namespace Game.Player
{
    [System.Serializable]
    public struct PlayerCombatData
    {
        [Header("Basic")]
        [SerializeField] private Bullet bulletPrefab;
        public readonly Bullet BulletPrefab => bulletPrefab;

        [SerializeField] private float bulletDamage;
        public readonly float BulletDamage => bulletDamage;

        [SerializeField] private float bulletSpeed;
        public readonly float BulletSpeed => bulletSpeed;
        [SerializeField] private float bulletRadius;
        public readonly float BulletRadius => bulletRadius;
        [SerializeField] private float bulletScale;
        public readonly float BulletScale => bulletScale;

        [SerializeField] private float fireRate;
        public readonly float FireRate => fireRate;

        [Header("Bullet Behaviour")]
        [SerializeField] private float shootDistance;
        public readonly float ShootDistance => shootDistance;

        [SerializeField] private LayerMask targetMask;
        public readonly LayerMask TargetMask => targetMask;

        [Header("Multi-Shot Settings")]
        [SerializeField] private int bulletCount;
        public readonly int BulletCount => bulletCount;

        [SerializeField] private float angleSpacing;
        public readonly float AngleSpacing => angleSpacing;
    }
}