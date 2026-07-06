using UnityEngine;
using Game.Entities;

namespace Game.Entities.Boss
{
    /// <summary>
    /// AI state activated when BossHealth reaches 0 HP.
    /// Immediately halts all movement and attacking, plays explosion/death effects, and destroys the boss GameObject.
    /// Place this state at Index 0 (Top Priority) in the stateList on BossBrain.
    /// </summary>
    public class BossDeathState : EntityState
    {
        [Header("Death Configuration")]
        [Tooltip("Delay in seconds before destroying the boss GameObject after death. Default: 3s.")]
        public float deathDelay = 3f;

        [Tooltip("Optional particle or explosion effect prefab spawned at boss position upon death.")]
        public GameObject deathEffectPrefab;

        [Tooltip("If checked, disables all colliders on death so player projectiles or body don't collide with the dying corpse.")]
        public bool disableCollidersOnDeath = true;

        private bool hasExecutedDeath = false;

        public override bool CheckConditions(EntityBrain brain)
        {
            BossHealth health = brain.GetComponent<BossHealth>();
            return health != null && health.IsDead;
        }

        public override void EnterState(EntityBrain brain)
        {
            if (hasExecutedDeath) return;
            hasExecutedDeath = true;

            Debug.Log($"[BossDeathState] Boss has died! Halting all movement and combat.");

            // Stop all movement immediately
            if (brain.EntityMovement != null)
            {
                brain.EntityMovement.SetMovement(Vector2.zero, 0f);
            }

            // Disable colliders so dead boss doesn't block player or take extra hits
            if (disableCollidersOnDeath)
            {
                Collider2D[] colliders = GetComponents<Collider2D>();
                foreach (var c in colliders)
                {
                    c.enabled = false;
                }
            }

            // Spawn visual death effect
            if (deathEffectPrefab != null)
            {
                GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, deathDelay + 1f);
            }

            // Schedule final GameObject destruction
            Destroy(gameObject, deathDelay);
        }

        public override void UpdateState(EntityBrain brain)
        {
            // Ensure boss stays completely frozen while dying
            if (brain.EntityMovement != null)
            {
                brain.EntityMovement.SetMovement(Vector2.zero, 0f);
            }
        }

        public override void ExitState(EntityBrain brain)
        {
        }
    }
}