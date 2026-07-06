using System.Collections.Generic;
using UnityEngine;
using Game.Generic;
using Game.Core.Effects;
using System.Collections;

namespace Game.Entities.Boss
{
    public class BossHealth : EntityHealth
    {
        [Header("Shield Visual Feedback")]
        [Tooltip("Animator yang akan memainkan animasi saat damage diblokir oleh shield Support Building.")]
        [SerializeField] private Animator shieldAnimator;

        [Tooltip("Nama parameter Trigger di Animator untuk animasi shield hit.")]
        [SerializeField] private string shieldHitTrigger = "ShieldHit";

        private List<BossSupportBuilding> activeSupportBuildings = new List<BossSupportBuilding>();

        public bool IsShielded => HasActiveDefense();
        public IReadOnlyList<BossSupportBuilding> ActiveSupportBuildings => activeSupportBuildings;

        [SerializeField] private BossHealthHUD bossHealthHUD;

        protected override void Awake()
        {
            base.Awake();
        }

        protected void OnEnable()
        {
            FindAnyObjectByType<BossHealthHUD>().FollowEvent(maxHealth);
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

            foreach (IVisualEffect visualEffect in visualEffects)
            {
                visualEffect.PlayEffect();
            }
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
            if (shieldAnimator != null && !string.IsNullOrEmpty(shieldHitTrigger))
            {
                shieldAnimator.SetTrigger(shieldHitTrigger);
            }
            else
            {
                Debug.LogWarning("[BossHealth] shieldAnimator belum di-assign atau trigger name kosong.");
            }
        }

        protected override void Die()
        {
            base.Die();
            StartCoroutine(DieCoroutine());
            Debug.Log($"[BossHealth] Heli Boss has been defeated!");
        }

        private IEnumerator DieCoroutine()
        {
            yield return new WaitForSeconds(2f);
            bossHealthHUD.gameObject.SetActive(false);
        }
    }
}