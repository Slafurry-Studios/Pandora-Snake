using UnityEngine;
using Game.Gameplay;

namespace Game.Entities.Boss
{
    public class BossBullet : Bullet
    {
        [Header("Boss Bullet Visuals & Effects")]
        [Tooltip("Optional particle effect spawned when this bullet hits a target or obstacle.")]
        public GameObject impactEffectPrefab;

        [Tooltip("Optional TrailRenderer to reset or manage when fired.")]
        public TrailRenderer trailRenderer;

        private void OnEnable()
        {
            if (trailRenderer != null)
            {
                trailRenderer.Clear();
            }
        }

        private void OnDisable()
        {
            SpawnImpactEffect();
        }

        private void SpawnImpactEffect()
        {
            if (impactEffectPrefab != null && gameObject.scene.isLoaded)
            {
                GameObject effect = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
                }
                else
                {
                    Destroy(effect, 2f);
                }
            }
        }
    }
}