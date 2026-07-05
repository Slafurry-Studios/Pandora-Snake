using System.Collections;
using UnityEngine;
using Game.Gameplay;

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
            if (finisherBulletPrefab == null || BulletManager.Instance == null || player == null)
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

                BulletManager.Instance.FireBullet(
                    finisherBulletPrefab,
                    spawnPos,
                    dirToPlayer,
                    bulletDamage,
                    bulletSpeed,
                    maxShootDistance,
                    playerMask,
                    bulletHitRadius,
                    false,  // isExplosive
                    false   // isRichochet
                );
            }
        }
    }
}