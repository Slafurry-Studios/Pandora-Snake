using UnityEngine;

namespace Game.AI
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string entityName;

        [Header("Stats")]
        public int health;
        public int threatPoint;
        public int growthPoint;

        public float movementSpeedMultiplier = 1f; 
    }
}