using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [System.Serializable]
    public struct DropItem
    {
        [Tooltip("The physical prefab to spawn in the world.")]
        public GameObject prefabToDrop;
        
        [Tooltip("How many of this prefab to drop.")]
        public int amount;
        
        [Tooltip("Chance to drop (0 to 1). 1 = 100% chance.")]
        [Range(0f, 1f)]
        public float dropChance;
    }

    [CreateAssetMenu(fileName = "NewLootDropData", menuName = "Game/Dropable/Loot Drop Data")]
    public class LootDropData : ScriptableObject
    {
        [Header("Loot Table")]
        public List<DropItem> drops = new List<DropItem>();
    }
}
