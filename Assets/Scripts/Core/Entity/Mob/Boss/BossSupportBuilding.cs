using UnityEngine;
using Game.Generic;

namespace Game.AI.Boss
{
    public class BossSupportBuilding : MonoBehaviour
    {
        [Header("Linked Boss Reference")]
        [Tooltip("Optional direct reference to the Heli Boss. If null, it will search for BossHealth in scene on Start.")]
        public BossHealth linkedBoss;

        [Header("Support Abilities Setup")]
        [Tooltip("If true, grants defense (damage immunity / shield) to the boss while this building stands.")]
        public bool providesDefense = true;
        
        [Tooltip("If true, periodically heals the linked boss.")]
        public bool providesHealing = true;
        public float healAmount = 2f;
        public float healInterval = 3f;

        private float nextHealTime;
        private bool isDestroyed = false;
        private Health buildingHealth;

        private void Start()
        {
            if (linkedBoss == null)
            {
                linkedBoss = FindObjectOfType<BossHealth>();
            }

            if (linkedBoss != null)
            {
                linkedBoss.RegisterSupportBuilding(this);
            }

            buildingHealth = GetComponent<Health>();
            if (buildingHealth != null)
            {
                buildingHealth.OnDied += OnBuildingDestroyed;
            }

            nextHealTime = Time.time + healInterval;
        }

        private void Update()
        {
            if (isDestroyed || linkedBoss == null || linkedBoss.IsDead)
                return;

            if (buildingHealth != null && buildingHealth.IsDead)
            {
                OnBuildingDestroyed();
                return;
            }

            if (providesHealing && Time.time >= nextHealTime)
            {
                linkedBoss.Heal(healAmount);
                nextHealTime = Time.time + healInterval;
            }

            ExecuteFutureSupportAbilities();
        }

        /// <summary>
        /// PLACEHOLDER: Override or extend this method to add future support building abilities
        /// (such as speed boost buffs, drone spawning, or EMP shockwaves).
        /// </summary>
        protected virtual void ExecuteFutureSupportAbilities()
        {
            // Future support logic hooks go here
        }

        private void OnBuildingDestroyed()
        {
            if (isDestroyed) return;
            isDestroyed = true;

            Debug.Log($"[BossSupportBuilding] {gameObject.name} destroyed! Removing support and landing spot from Heli Boss.");

            if (linkedBoss != null)
            {
                linkedBoss.UnregisterSupportBuilding(this);
            }
        }

        private void OnDestroy()
        {
            if (buildingHealth != null)
            {
                buildingHealth.OnDied -= OnBuildingDestroyed;
            }

            OnBuildingDestroyed();
        }
    }
}