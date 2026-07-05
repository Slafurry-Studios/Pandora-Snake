using Game.Manager;
using Game.Upgrade;
using TMPro;
using UnityEngine;

namespace Game.UI.HUD
{
    public class UpgradeHUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject UpgradePanel;
        [SerializeField] private UpgradeCardHUD card1;
        [SerializeField] private UpgradeCardHUD card2;
        [SerializeField] private UpgradeCardHUD card3;

        void Start()
        {
            GameManager.Instance.upgradeManager.OnUpgrade += Show;
            GameManager.Instance.upgradeManager.SetCard1 += SetCard1;
            GameManager.Instance.upgradeManager.SetCard2 += SetCard2;
            GameManager.Instance.upgradeManager.SetCard3 += SetCard3;

        }

        public void Show(bool status)
        {
            UpgradePanel.SetActive(status);
            Time.timeScale = status ? 0f : 1f;
        }

        private void SetCard1(UpgradeCard card)
        {
            card1.SetUpgradeCard(card);
        }
        private void SetCard2(UpgradeCard card)
        {
            card2.SetUpgradeCard(card);
        }
        private void SetCard3(UpgradeCard card)
        {
            card3.SetUpgradeCard(card);
        }
    }
}