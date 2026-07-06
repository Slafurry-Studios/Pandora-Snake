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

        public void Initialize()
        {
            Player player = GetComponentInParent<Player>();
            playerTail = player.PlayerTail;
        }

        public void AddGrowPoints(int amount)
        {
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
