using System.Data.SqlTypes;
using Slafurry.Utils.Attributes;
using UnityEngine;

namespace Game.Player
{
    [GameAssetCreator("Player", "PlayerStats", order: 2)]
    public class PlayerData : ScriptableObject
    {
        [Header("Stats")]
        [SerializeField] private float playerHealth;
        public float PlayerHealth => playerHealth;


        [Header("Movement")]
        [SerializeField] private float playerStamina;
        public float PlayerStamina => playerStamina;
        [SerializeField] private float playerSpeed;
        public float PlayerSpeed => playerSpeed;
        [SerializeField] private PlayerMovementData playerMovementData;
        public PlayerMovementData PlayerMovementData => playerMovementData;


        [Header("Combat")]
        [SerializeField] private PlayerCombatData combatData;
        public PlayerCombatData CombatData => combatData;

        [Header("Audio")]
        [SerializeField] private string takeDamageSFX;
        public string TakeDamageSFX => takeDamageSFX;

        [SerializeField] private string shootSFX;
        public string ShootSFX => shootSFX;
    }
}