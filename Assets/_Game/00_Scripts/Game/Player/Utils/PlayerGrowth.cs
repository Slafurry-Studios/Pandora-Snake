using UnityEngine;

namespace Game.Player
{
    public class PlayerGrowth : MonoBehaviour
    {
        [Header("Growth Settings")]
        [Tooltip("How many grow points are needed to grow one tail segment.")]
        [SerializeField] private int growPointsPerTail = 5;
        [SerializeField] private int currentGrowPoints = 0;
        private int accumulatedGrowPoint = 0;
        private SnakeTailManager playerTail;
        private bool initialized;

        public void Initialize(SnakeTailManager playerTail)
        {
            this.playerTail = playerTail;
            initialized = true;
        }

        public void AddGrowPoints(int amount)
        {
            if (!initialized) return;
            
            currentGrowPoints += amount;
            accumulatedGrowPoint += amount;
            
            while (currentGrowPoints >= growPointsPerTail)
            {
                currentGrowPoints -= growPointsPerTail;
                playerTail.Grow(1);
            }
        }
        
        public int GetCurrentGrowPoints() => accumulatedGrowPoint;
        public int GetGrowPointsPerTail() => growPointsPerTail;
    }
}
