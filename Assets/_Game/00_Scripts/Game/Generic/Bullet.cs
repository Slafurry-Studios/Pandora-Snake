using Slafurry.Utils.Pooling;
using UnityEngine;

namespace Game.Gameplay
{
    public class Bullet : MonoBehaviour, IPoolable
    {
        public int prefabID { get; private set; }
        public float damage { get; private set; }
        public float speed { get; private set; }
        public float maxDistance { get; private set; }
        public LayerMask targetMask { get; private set; }
        public float hitRadius { get; private set; }

        public Vector2 startPosition;
        public Vector2 direction;
        public bool isExplosive { get; private set; }
        public bool isRichochet { get; private set; }

        public void Setup(int pID, float dmg, float spd, float dist, Vector2 dir, LayerMask mask, float radius, bool isExplosive, bool isRichochet)
        {
            prefabID = pID;
            damage = dmg;
            speed = spd;
            maxDistance = dist;
            direction = dir.normalized;
            targetMask = mask;
            hitRadius = radius;
            this.isExplosive = isExplosive;
            this.isRichochet = isRichochet;

            startPosition = transform.position;

            if (direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        public void OnSpawn() { }

        public void OnDespawn()
        {
            direction = Vector2.zero;
        }
    }
}
