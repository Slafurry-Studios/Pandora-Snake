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
        [SerializeField] private PlayerMovementData playerMovementData;
        public PlayerMovementData PlayerMovementData => playerMovementData;

        [Header("Combat")]
        [SerializeField] private PlayerCombatData combatData;
        public PlayerCombatData CombatData => combatData;

        [Header("Collision")]
        [SerializeField] private float collisionDamage;
        public float CollisionDamage => collisionDamage;

        [Header("Audio")]
        [SerializeField] private string takeDamageSFX;
        public string TakeDamageSFX => takeDamageSFX;

    }
}