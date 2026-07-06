using UnityEngine;
using UnityEngine.Events;

namespace Game.Player
{
    public class PlayerDeath : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject playerDizzyAnimator;
        [SerializeField] private SpriteRenderer headSprite;

        [SerializeField] private Sprite deathSprite;
        [SerializeField] private UnityEvent[] OnDeath;
        private PlayerHealth playerHealth;
        private PlayerStamina playerStamina;
        private PlayerMovement playerMovement;
        private Rigidbody2D rb;
        private bool initialized;

        public void Initialize(PlayerHealth playerHealth, PlayerStamina playerStamina, PlayerMovement playerMovement, Rigidbody2D rb)
        {
            this.playerHealth = playerHealth;
            this.playerStamina = playerStamina;
            this.playerMovement = playerMovement;
            this.rb = rb;

            initialized = true;
        }

        public void Death()
        {
            if (!initialized) return;

            playerHealth.SetHealth(0f);
            headSprite.sprite = deathSprite;
            PlayerManager.Instance.HideHUD();

            if (playerDizzyAnimator != null)
            {
                playerDizzyAnimator.gameObject.SetActive(true);
            }

            playerStamina.SetStamina(0f);
            playerMovement.enabled = false;

            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            int randomIndex = Random.Range(0, OnDeath.Length);
            OnDeath[randomIndex]?.Invoke();
        }
    }
}