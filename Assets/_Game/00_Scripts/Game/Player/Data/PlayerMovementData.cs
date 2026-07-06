using UnityEngine;

namespace Game.Player
{
    [System.Serializable]
    public struct PlayerMovementData
    {
        [SerializeField] private float playerStamina;
        public float PlayerStamina => playerStamina;
        [SerializeField] private float playerSpeed;
        public float PlayerSpeed => playerSpeed;
        [SerializeField] private float playerSprintMultiplier;
        public float PlayerSpeedMultiplier => playerSprintMultiplier;

        [SerializeField] private float staminaDrainRate;
        public float StaminaDrainRate => staminaDrainRate;

        [SerializeField] private float staminaRegenRate;
        public float StaminaRegenRate => staminaRegenRate;

        [SerializeField] private float staminaRegenDelay;
        public float StaminaRegenDelay => staminaRegenDelay;
    }

}