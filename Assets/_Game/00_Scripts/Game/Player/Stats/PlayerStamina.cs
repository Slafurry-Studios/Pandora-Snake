using System;
using UnityEngine;

namespace Game.Player
{
    public class PlayerStamina : MonoBehaviour
    {
        public event Action<float> OnStaminaPctChanged;

        private float maxStamina;
        private float staminaDrainRate;
        private float staminaRegenRate;
        private float staminaRegenDelay;


        private float currentStamina;
        private float regenTimer;
        private bool isExhausted;

        public bool CanSprint => !isExhausted && currentStamina > 0f;

        public void Initialize(PlayerMovementData movementData)
        {
            maxStamina = movementData.PlayerStamina;
            staminaDrainRate = movementData.StaminaDrainRate;
            staminaRegenRate = movementData.StaminaRegenRate;
            staminaRegenDelay = movementData.StaminaRegenDelay;
            
            currentStamina = maxStamina;

            OnStaminaPctChanged?.Invoke(currentStamina / maxStamina);
        }

        public void ResetExhaustion()
        {
            isExhausted = false;
        }

        public void Drain(float deltaTime)
        {
            float previous = currentStamina;

            currentStamina = Mathf.Max(currentStamina - staminaDrainRate * deltaTime, 0f);
            regenTimer = staminaRegenDelay;

            if (currentStamina <= 0f)
                isExhausted = true;

            if (currentStamina != previous)
                OnStaminaPctChanged?.Invoke(currentStamina / maxStamina);
        }

        public void Regen(float deltaTime)
        {
            float previous = currentStamina;

            if (regenTimer > 0f)
            {
                regenTimer -= deltaTime;
            }
            else if (currentStamina < maxStamina)
            {
                currentStamina = Mathf.Min(currentStamina + staminaRegenRate * deltaTime, maxStamina);
            }

            if (currentStamina != previous)
                OnStaminaPctChanged?.Invoke(currentStamina / maxStamina);
        }

        public float GetStaminaNormalized() => currentStamina / maxStamina;

        public void SetStamina(float value)
        {
            float previous = currentStamina;
            currentStamina = Mathf.Clamp(value, 0f, maxStamina);

            if (currentStamina != previous)
                OnStaminaPctChanged?.Invoke(currentStamina / maxStamina);
        }
        public void InfiniteCardio()
        {
            maxStamina *= 1.1f;
        }
    }
}