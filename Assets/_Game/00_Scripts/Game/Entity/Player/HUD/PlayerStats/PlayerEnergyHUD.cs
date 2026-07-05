using UnityEngine;
using UnityEngine.UI;
using Game.Player;

namespace Game.UI.HUD
{

    public class PlayerEnergyHUD : MonoBehaviour
    {
        [SerializeField] private Slider EnergySlider;

        private PlayerMovement playerMovement;

        void Start()
        {
            playerMovement = FindAnyObjectByType<PlayerMovement>();

            if (playerMovement != null) playerMovement.OnStaminaPctChanged += UpdateBar;
        }

        private void UpdateBar(float amount)
        {
            EnergySlider.value = amount;
        }
    }
}