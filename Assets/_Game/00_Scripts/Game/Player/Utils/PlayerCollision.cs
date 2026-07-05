using UnityEngine;

namespace Game.Player
{
    public class PlayerCollision : MonoBehaviour
    {
        [SerializeField] private float collisionDmg = 1f;
        private Player player;

        private PlayerHealth playerHealth;
        private PlayerDeath playerDeath;

        public void Initialize()
        {
            player = GetComponentInParent<Player>();
            playerDeath = player.PlayerDeath;
            playerHealth = player.PlayerHealth;
        }
        
        public void CheckCollision(GameObject obj)
        {
            if (playerHealth.IsDead)
                return;

            if (obj.CompareTag("Body"))
            {
                playerDeath.Death();
            }

            if (obj.CompareTag("Building"))
            {
                obj.GetComponent<BuildingHealth>().TakeDamage(999999999f);
                playerHealth.TakeDamage(collisionDmg);
            }
        }

        public void SafetyFirst()
        {
            collisionDmg = collisionDmg / 2;
        }
    }
}