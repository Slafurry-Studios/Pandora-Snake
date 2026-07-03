using System;
using Game.Manager;
using Game.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.HUD
{

    public class UpgradeCardHUD : MonoBehaviour
    {
        private UpgradeCard upgradeCard;
        [SerializeField] private Image cardBg;
        [SerializeField] private Button cardButton;

        private void OnEnable()
        {
            if (cardButton != null)
                cardButton.onClick.AddListener(SetOnClick);
        }

        private void OnDisable()
        {
            if (cardButton != null)
                cardButton.onClick.RemoveListener(SetOnClick);
        }

        public void SetUpgradeCard(UpgradeCard upgradeCard)
        {
            this.upgradeCard = upgradeCard;
            SetUI();
        }
        private void SetUI()
        {
            cardBg.sprite = upgradeCard.UpgradeCardData.UpgradeBg;
        }

        private void SetOnClick()
        {
            upgradeCard?.OnSelected();
            FindAnyObjectByType<PlayerBuffHUD>().AddBuff();
            GetComponentInParent<UpgradeHUD>().Show(false);
        }
    }
}