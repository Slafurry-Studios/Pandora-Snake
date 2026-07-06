using Game.Gameplay;
using Game.Generic;
using Slafurry.System.VFX;
using UnityEngine;

namespace Game
{
    public class BulletExplodeHelper : MonoBehaviour
    {
        [SerializeField] private string explosionVFXKey;
        [SerializeField] private float explosionRadius = 3f;
        [SerializeField] private float explosionForce = 10f;
        private Collider2D[] explosionResults = new Collider2D[32];

        public void Explode(Bullet b)
        {
            Vector2 explosionPos = b.transform.position;
            VFX.Play(explosionVFXKey, explosionPos);

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

    }
}