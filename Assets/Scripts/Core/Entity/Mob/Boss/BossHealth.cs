using System.Collections.Generic;
using UnityEngine;
using Game.Generic;

namespace Game.AI.Boss
{
    public class BossHealth : Health
    {
        [Header("Shield Visual Feedback")]
        [Tooltip("Optional visual effect prefab spawned whenever damage is blocked by an active Support Building shield.")]
        public GameObject shieldHitEffectPrefab;

        private List<BossSupportBuilding> activeSupportBuildings = new List<BossSupportBuilding>();

        public bool IsShielded => HasActiveDefense();
        public IReadOnlyList<BossSupportBuilding> ActiveSupportBuildings => activeSupportBuildings;

        protected override void Awake()
        {
            base.Awake();
        }

        public void RegisterSupportBuilding(BossSupportBuilding building)
        {
            if (building != null && !activeSupportBuildings.Contains(building))
            {
                activeSupportBuildings.Add(building);
                Debug.Log($"[BossHealth] Registered Support Building: {building.gameObject.name}. Total Active: {activeSupportBuildings.Count}");
            }
        }

        public void UnregisterSupportBuilding(BossSupportBuilding building)
        {
            if (activeSupportBuildings.Contains(building))
            {
                activeSupportBuildings.Remove(building);
                Debug.Log($"[BossHealth] Unregistered Support Building: {building.gameObject.name}. Total Remaining: {activeSupportBuildings.Count}");
            }
        }

        private bool HasActiveDefense()
        {
            for (int i = activeSupportBuildings.Count - 1; i >= 0; i--)
            {
                if (activeSupportBuildings[i] == null)
                {
                    activeSupportBuildings.RemoveAt(i);
                    continue;
                }

                if (activeSupportBuildings[i].providesDefense)
                {
                    return true;
                }
            }
            return false;
        }

        public override void TakeDamage(float amount)
        {
            if (isDead || amount <= 0f)
                return;

            if (HasActiveDefense())
            {
                Debug.Log($"[BossHealth] Damage ({amount}) BLOCKED by active Support Building defense shield!");
                PlayShieldHitEffect();
                return;
            }

            base.TakeDamage(amount);
        }

        /// <summary>
        /// Bypasses any active Support Building defense shield and deals damage directly. Intended for testing and debug triggers.
        /// </summary>
        public void BypassShieldDamage(float amount)
        {
            if (isDead || amount <= 0f)
                return;

            Debug.Log($"[BossHealth] Bypassed defense shields to apply {amount} debug damage!");
            base.TakeDamage(amount);
        }

        private void PlayShieldHitEffect()
        {
            if (shieldHitEffectPrefab != null)
            {
                GameObject effect = Instantiate(shieldHitEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
        }

        protected override void Die()
        {
            base.Die();
            Debug.Log($"[BossHealth] Heli Boss has been defeated!");
        }
    }
}