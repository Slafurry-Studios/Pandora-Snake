using UnityEngine;
using Game.AI;
using Game.AI.Boss;

namespace Game.Temp
{
    public class BossLandingTester : MonoBehaviour
    {
        [Header("Target Boss Reference")]
        [Tooltip("Optional reference to the BossHealth component. If left blank, automatically searches the GameObject or scene.")]
        public BossHealth targetBossHealth;

        [Header("Keyboard Controls")]
        [Tooltip("Press this keyboard key during Play Mode to instantly reduce boss HP below the landing threshold and trigger landing!")]
        public KeyCode triggerKey = KeyCode.L;

        private void Update()
        {
            if (Input.GetKeyDown(triggerKey))
            {
                TriggerLandingViaDamage();
            }
        }

        private BossHealth GetBossHealth()
        {
            if (targetBossHealth != null) return targetBossHealth;
            targetBossHealth = GetComponent<BossHealth>();
            if (targetBossHealth != null) return targetBossHealth;
            return Object.FindObjectOfType<BossHealth>();
        }

        [ContextMenu("Trigger Landing Via Damage")]
        public void TriggerLandingViaDamage()
        {
            BossHealth health = GetBossHealth();
            if (health == null)
            {
                Debug.LogWarning("[BossLandingTester] Could not find a BossHealth component in the scene!");
                return;
            }

            BossLandState landState = health.GetComponent<BossLandState>();
            float thresholdPct = (landState != null) ? landState.landingHpThresholdPct : 0.5f;

            float targetHp = (health.MaxHealth * thresholdPct) - 1f;
            if (targetHp < 1f) targetHp = 1f;

            float damageNeeded = health.CurrentHealth - targetHp;
            if (damageNeeded > 0)
            {
                // Use BypassShieldDamage so testing works even when Support Building invincibility shield is active!
                health.BypassShieldDamage(damageNeeded);
                Debug.Log($"[BossLandingTester] Bypassed shield & dealt {damageNeeded} damage! Boss HP is now {health.CurrentHealth}/{health.MaxHealth} (Below threshold {thresholdPct * 100}%). Watch it retreat to the Support Building!");
            }
            else
            {
                Debug.Log($"[BossLandingTester] Boss HP ({health.CurrentHealth}) is already below the landing threshold ({thresholdPct * 100}%)!");
            }
        }
    }
}
