using UnityEngine;

namespace Game.Player
{
    public class PlayerCollision : MonoBehaviour
    {
        private float collisionDmg;
        private PlayerHealth playerHealth;
        private PlayerDeath playerDeath;
        private bool initialized;
        public void Initialize(PlayerHealth playerHealth, PlayerDeath playerDeath, float collisionDmg)
        {
            this.playerDeath = playerDeath;
            this.playerHealth = playerHealth;
            this.collisionDmg = collisionDmg;
            
            initialized = true;
        }
        
        public void CheckCollision(GameObject obj)
        {
            if (!initialized) return;

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