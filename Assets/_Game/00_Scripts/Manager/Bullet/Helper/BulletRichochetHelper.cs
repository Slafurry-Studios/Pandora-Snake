using Game.Gameplay;
using UnityEngine;

namespace Game
{
    public class BulletRichochetHelper : MonoBehaviour
    {
        [SerializeField] private int maxBounces = 1;
        public int MaxBounces => maxBounces;

        public void Ricochet(ActiveBulletData data, Collider2D hitCollider)
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

            float angle = Mathf.Atan2(reflected.y, reflected.x) * Mathf.Rad2Deg;
            b.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}