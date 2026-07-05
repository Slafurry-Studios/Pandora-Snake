using UnityEngine;
using UnityEngine.UI;
using Game.Player;

namespace Game.UI.HUD
{

    public class PlayerEnergyHUD : MonoBehaviour
    {
        [SerializeField] private Slider EnergySlider;

        private PlayerStamina playerStamina;

        void Start()
        {
            playerStamina = FindAnyObjectByType<PlayerStamina>();

            if (playerStamina != null) playerStamina.OnStaminaPctChanged += UpdateBar;
        }

        private void UpdateBar(float amount)
        {
            EnergySlider.value = amount;
        }
    }
}