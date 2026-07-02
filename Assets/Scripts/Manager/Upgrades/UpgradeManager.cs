using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Manager;

namespace Game.Upgrade
{
    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField] private UpgradeCard[] upgradeCards;

        private readonly HashSet<UpgradeCard> currentUpgrades = new();

        public Action<UpgradeCard> SetCard1;
        public Action<UpgradeCard> SetCard2;
        public Action<UpgradeCard> SetCard3;
        public Action<bool> OnUpgrade;

        private void Start()
        {
            GameManager.Instance.subsManager.OnCurrentSubsStateChanged += SetNewUpgrades;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.subsManager != null)
            {
                GameManager.Instance.subsManager.OnCurrentSubsStateChanged -= SetNewUpgrades;
            }
        }

        private void SetNewUpgrades(int threshold)
        {
            // Debug.Log("[UpgradeManager] listened");
            List<UpgradeCard> rolled = RollUniqueCards(3);

            if (rolled.Count == 0)
                return;

            if (rolled.Count > 0)
                SetCard1?.Invoke(rolled[0]);

            if (rolled.Count > 1)
                SetCard2?.Invoke(rolled[1]);

            if (rolled.Count > 2)
                SetCard3?.Invoke(rolled[2]);

            OnUpgrade?.Invoke(true);
        }

        private List<UpgradeCard> RollUniqueCards(int count)
        {
            List<UpgradeCard> pool = upgradeCards
                .Where(CanAppear)
                .ToList();

            if (pool.Count == 0)
            {
                Debug.LogWarning("Tidak ada upgrade yang memenuhi prerequisite.");
                return new List<UpgradeCard>();
            }

            count = Mathf.Min(count, pool.Count);

            List<UpgradeCard> result = new();

            for (int i = 0; i < count; i++)
            {
                UpgradeCard picked = WeightedPick(pool);

                result.Add(picked);
                pool.Remove(picked);
            }

            return result;
        }

        private bool CanAppear(UpgradeCard card)
        {
            // Jangan munculkan lagi upgrade yang sudah dimiliki
            if (currentUpgrades.Contains(card))
                return false;

            // Tidak punya prerequisite
            if (card.Prequesities == null || card.Prequesities.Length == 0)
                return true;

            // Semua prerequisite harus dimiliki
            return card.Prequesities.All(p => currentUpgrades.Contains(p));
        }

        private UpgradeCard WeightedPick(List<UpgradeCard> pool)
        {
            float totalWeight = pool.Sum(c => Mathf.Max(c.UpgradeCardData.Weight, 0));

            if (totalWeight <= 0)
                return pool[UnityEngine.Random.Range(0, pool.Count)];

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float cumulative = 0;

            foreach (UpgradeCard card in pool)
            {
                cumulative += Mathf.Max(card.UpgradeCardData.Weight, 0);

                if (roll <= cumulative)
                    return card;
            }

            return pool[^1];
        }

        public void SelectUpgrade(UpgradeCard card)
        {
            if (card == null)
                return;

            if (currentUpgrades.Add(card))
            {
                card.OnSelected();
            }

            OnUpgrade?.Invoke(false);
        }

        public bool HasUpgrade(UpgradeCard card)
        {
            return currentUpgrades.Contains(card);
        }
    }
}