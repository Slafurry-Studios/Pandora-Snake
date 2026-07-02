using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Game.Core;
using Game.Generic;

namespace Game.Gameplay
{
    public class BulletManager : MonoBehaviour
    {
        public static BulletManager Instance { get; private set; }

        [Header("Global Bullet Effects")]
        [SerializeField] private GameObject hitVFXPrefab;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private float explosionRadius = 3f;
        [SerializeField] private float explosionForce = 10f;
        [SerializeField] private int maxBounces = 3;

        private class ActiveBulletData
        {
            public Bullet bullet;
            public int bouncesLeft;
        }

        private Dictionary<int, ObjectPool<Bullet>> pools = new Dictionary<int, ObjectPool<Bullet>>();
        private List<ActiveBulletData> activeBullets = new List<ActiveBulletData>();

        private Collider2D[] hitResults = new Collider2D[1];
        private Collider2D[] explosionResults = new Collider2D[32];

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void FireBullet(Bullet prefab, Vector2 startPos, Vector2 direction, float damage, float speed, float maxDistance, LayerMask targetMask, float hitRadius, bool isExplosive, bool isRichochet)
        {
            if (prefab == null) return;

            int prefabID = prefab.gameObject.GetInstanceID();

            if (!pools.ContainsKey(prefabID))
            {
                CreatePoolForPrefab(prefab, prefabID);
            }

            Bullet bullet = pools[prefabID].Get();
            bullet.transform.position = startPos;
            bullet.Setup(prefabID, damage, speed, maxDistance, direction, targetMask, hitRadius, isExplosive, isRichochet);

            activeBullets.Add(new ActiveBulletData { bullet = bullet, bouncesLeft = maxBounces });
        }

        private void CreatePoolForPrefab(Bullet prefab, int prefabID)
        {
            ObjectPool<Bullet> newPool = new ObjectPool<Bullet>(
                createFunc: () => Instantiate(prefab, transform),
                actionOnGet: (b) => b.gameObject.SetActive(true),
                actionOnRelease: (b) => b.gameObject.SetActive(false),
                actionOnDestroy: (b) => Destroy(b.gameObject),
                collectionCheck: true,
                defaultCapacity: 50,
                maxSize: 1000
            );
            pools.Add(prefabID, newPool);
        }

        private void Update()
        {
            for (int i = activeBullets.Count - 1; i >= 0; i--)
            {
                ActiveBulletData data = activeBullets[i];
                Bullet b = data.bullet;

                b.transform.position += (Vector3)(b.direction * b.speed * Time.deltaTime);

                int hits = Physics2D.OverlapCircleNonAlloc(b.transform.position, b.hitRadius, hitResults, b.targetMask);
                if (hits > 0)
                {
                    Collider2D hitCollider = hitResults[0];
                    Health health = hitCollider.GetComponent<Health>();

                    SpawnHitObject(b, hitCollider);

                    if (b.isExplosive)
                    {
                        Explode(b);
                        ReturnBullet(data, i);
                        continue;
                    }

                    if (health != null && !health.IsDead)
                    {
                        health.TakeDamage(b.damage);
                        ReturnBullet(data, i);
                    }
                    else if (b.isRichochet && data.bouncesLeft > 0)
                    {
                        Ricochet(data, hitCollider);
                    }
                    else
                    {
                        ReturnBullet(data, i);
                    }
                    continue;
                }

                if (Vector2.Distance(b.startPosition, b.transform.position) >= b.maxDistance)
                {
                    if (b.isExplosive)
                    {
                        Explode(b);
                    }
                    ReturnBullet(data, i);
                }
            }
        }

        private void Ricochet(ActiveBulletData data, Collider2D hitCollider)
        {
            Bullet b = data.bullet;
            Vector2 hitPoint = hitCollider.ClosestPoint(b.transform.position);
            Vector2 normal = ((Vector2)b.transform.position - hitPoint).normalized;

            if (normal.sqrMagnitude < 0.0001f)
            {
                normal = -b.direction;
            }

            Vector2 reflected = Vector2.Reflect(b.direction, normal).normalized;
            b.direction = reflected;
            data.bouncesLeft--;

            b.transform.position += (Vector3)(normal * 0.05f);
            b.startPosition = b.transform.position;
        }

        private void SpawnHitObject(Bullet b, Collider2D hitCollider)
        {
            if (hitVFXPrefab == null) return;

            Vector2 hitPoint = hitCollider.ClosestPoint(b.transform.position);
            Instantiate(hitVFXPrefab, hitPoint, Quaternion.identity);
        }

        private void Explode(Bullet b)
        {
            Vector2 explosionPos = b.transform.position;

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, explosionPos, Quaternion.identity);
            }

            int count = Physics2D.OverlapCircleNonAlloc(explosionPos, explosionRadius, explosionResults, b.targetMask);
            for (int j = 0; j < count; j++)
            {
                Collider2D col = explosionResults[j];
                if (col == null) continue;

                Vector2 toTarget = (Vector2)col.transform.position - explosionPos;
                float distance = toTarget.magnitude;
                float falloff = Mathf.Clamp01(1f - (distance / explosionRadius));

                Health health = col.GetComponent<Health>();
                if (health != null && !health.IsDead)
                {
                    health.TakeDamage(b.damage * falloff);
                }

                Rigidbody2D rb = col.attachedRigidbody;
                if (rb != null)
                {
                    Vector2 forceDir = distance > 0.0001f ? toTarget.normalized : Random.insideUnitCircle.normalized;
                    rb.AddForce(forceDir * explosionForce * falloff, ForceMode2D.Impulse);
                }
            }
        }

        private void ReturnBullet(ActiveBulletData data, int index)
        {
            activeBullets.RemoveAt(index);
            if (pools.TryGetValue(data.bullet.prefabID, out ObjectPool<Bullet> pool))
            {
                pool.Release(data.bullet);
            }
            else
            {
                data.bullet.gameObject.SetActive(false); // Fallback
            }
        }
    }
}