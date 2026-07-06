using System.Collections.Generic;
using UnityEngine;
using Game.Generic;
using Slafurry.Utils.Pooling;
using Game.Gameplay;
using System.Collections;
using Slafurry.Core.Abstract;
using Slafurry.System.VFX;

namespace Game
{
    public class BulletManager : Manager
    {
        [Header("Global Bullet Effects")]
        [SerializeField] private string hitVFXKey;

        private Dictionary<int, GenericPool<Bullet>> pools = new Dictionary<int, GenericPool<Bullet>>();
        private List<ActiveBulletData> activeBullets = new List<ActiveBulletData>();

        private Collider2D[] hitResults = new Collider2D[1];
        private BulletExplodeHelper bulletExplodeHelper;
        private BulletRichochetHelper bulletRichochetHelper;

        protected override void RegisterToGameManager()
        {
            GameManager.Instance.RegisterBulletManager(this);
        }

        protected override void UnregisterFromGameManager()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.UnregisterBulletManager(this);
        }

        public override IEnumerator Initialize()
        {
            yield return null;
        }

        protected override void OnPostInitialize()
        {
            bulletExplodeHelper = GetComponentInChildren<BulletExplodeHelper>();
            bulletRichochetHelper = GetComponentInChildren<BulletRichochetHelper>();
        }

        public void FireBullet(BulletData data)
        {
            if (data.prefab == null) return;

            int prefabID = data.prefab.gameObject.GetInstanceID();

            if (!pools.TryGetValue(prefabID, out GenericPool<Bullet> pool))
            {
                pool = new GenericPool<Bullet>(data.prefab, transform, defaultCapacity: 50, maxSize: 1000);
                pools.Add(prefabID, pool);
            }

            Bullet bullet = pool.Get();
            bullet.transform.position = data.startPos;
            bullet.transform.localScale = Vector3.one * data.scale;
            bullet.Setup(prefabID, data.damage, data.speed, data.maxDistance, data.direction, data.targetMask, data.hitRadius, data.isExplosive, data.isRichochet);

            activeBullets.Add(new ActiveBulletData
            {
                bullet = bullet,
                bouncesLeft = data.isRichochet ? bulletRichochetHelper.MaxBounces : 0
            });
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
                    Health health = hitCollider.GetComponentInChildren<Health>();

                    SpawnHitObject(b, hitCollider);

                    if (b.isExplosive)
                    {
                        bulletExplodeHelper.Explode(b);
                        ReturnBullet(data, i);
                        continue;
                    }

                    if (health != null && !health.IsDead)
                    {
                        health.TakeDamage(b.damage);
                    }

                    if (b.isRichochet && data.bouncesLeft > 0)
                    {
                        bulletRichochetHelper.Ricochet(data, hitCollider);
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
                        bulletExplodeHelper.Explode(b);
                    }
                    ReturnBullet(data, i);
                }
            }
        }

        private void SpawnHitObject(Bullet b, Collider2D hitCollider)
        {
            Vector2 hitPoint = hitCollider.ClosestPoint(b.transform.position);
            VFX.Play(hitVFXKey, hitPoint);
        }

        private void ReturnBullet(ActiveBulletData data, int index)
        {
            activeBullets.RemoveAt(index);
            if (pools.TryGetValue(data.bullet.prefabID, out GenericPool<Bullet> pool))
            {
                pool.Release(data.bullet);
            }
            else
            {
                data.bullet.gameObject.SetActive(false);
            }
        }


    }
}