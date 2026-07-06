using System.Collections;
using UnityEngine;
using Game.Gameplay;
using Game.Managerd;

namespace Game.Gameplay
{
    public class PlayerFinisher : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Bullet finisherBulletPrefab;
        [SerializeField] private Transform player;
        [SerializeField] private LayerMask playerMask;

        [Header("Ring Settings")]
        [SerializeField] private int bulletsPerRing = 24;
        [SerializeField] private float ringRadius = 8f;
        [SerializeField] private float bulletSpeed = 15f;
        [SerializeField] private float bulletDamage = 999f;
        [SerializeField] private float bulletHitRadius = 0.2f;
        [SerializeField] private float maxShootDistance = 20f;

        [Header("Sequence Settings")]
        [SerializeField] private int ringCount = 3;
        [SerializeField] private float delayBetweenRings = 0.4f;
        [SerializeField] private float holdBeforeFire = 0.5f;

        public void TriggerFinisher()
        {
            StartCoroutine(FinisherSequence());
        }

        private IEnumerator FinisherSequence()
        {
            yield return new WaitForSeconds(holdBeforeFire);

            for (int ring = 0; ring < ringCount; ring++)
            {
                SpawnBulletRing();
                yield return new WaitForSeconds(delayBetweenRings);
            }
        }

        private void SpawnBulletRing()
        {
            if (finisherBulletPrefab == null || GameManager.Bullet == null || player == null)
                return;

            Vector2 playerPos = player.position;

            for (int i = 0; i < bulletsPerRing; i++)
            {
                float angle = (360f / bulletsPerRing) * i;
                Vector2 offset = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                ) * ringRadius;

                Vector2 spawnPos = playerPos + offset;
                Vector2 dirToPlayer = (playerPos - spawnPos).normalized;


                BulletFireData bulletFireData = new BulletFireData
                {
                    prefab = finisherBulletPrefab,
                    startPos = spawnPos,
                    direction = dirToPlayer,
                    damage = bulletDamage,
                    speed = bulletSpeed,
                    targetMask = playerMask,
                    maxDistance = maxShootDistance,
                    hitRadius = bulletHitRadius,
                };

                GameManager.Bullet.FireBullet(bulletFireData);
            }
        }
    }
}