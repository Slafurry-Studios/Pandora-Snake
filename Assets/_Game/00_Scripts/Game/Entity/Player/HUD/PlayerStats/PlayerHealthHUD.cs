using System;
using Game.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.HUD
{

    public class PlayerHealthHUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image HealthUI;
        [SerializeField] private Image Health2UI;

        [Header("Assets")]
        [SerializeField] private Sprite[] HealthAsset = new Sprite[6];
        [SerializeField] private Sprite[] Health2Asset = new Sprite[6];

        private PlayerHealth playerHealth;

        void Start()
        {
            playerHealth = FindAnyObjectByType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += SetHealth;
                playerHealth.OnDied += SetEmpty;
            }
        }

        private void SetHealth(float value)
        {
            int amount = Math.Clamp(Mathf.RoundToInt(value), 0, 10);

            if (amount <= 5)
            {
                HealthUI.sprite = HealthAsset[amount];
                Health2UI.sprite = Health2Asset[0];
            }
            else
            {
                HealthUI.sprite = HealthAsset[5];
                Health2UI.sprite = Health2Asset[amount - 5];
            }
        }

        private void SetEmpty()
        {
            HealthUI.sprite = HealthAsset[0];
            Health2UI.sprite = Health2Asset[0];
        }
    }
}