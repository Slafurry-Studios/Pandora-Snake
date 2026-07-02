using System;
using Game.Core.Effects;
using Game.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerHealth : Health
    {
        [Header("Death Settings")]
        [SerializeField] private Animator playerDizzyAnimator;
        [SerializeField] private PlayerMovement playerMovement;

        [SerializeField] private float collisionDmg = 1f;
        public IVisualEffect[] visualEffects;

        private RigidbodyConstraints2D originalConstraints;
        private bool vipSprint;

        protected override void Awake()
        {
            base.Awake();

            if (playerMovement != null)
            {
                Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    originalConstraints = rb.constraints;
                }
            }

            visualEffects = GetComponentsInChildren<IVisualEffect>();
        }

        protected override void Die()
        {
            currentHealth = 0f;
            base.Die();

            if (playerDizzyAnimator != null)
            {
                playerDizzyAnimator.gameObject.SetActive(true);
                playerDizzyAnimator.SetTrigger("Collide");
            }

            if (playerMovement != null)
            {
                playerMovement.SetStamina(0f);
                playerMovement.enabled = false;

                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                }

                if (playerDizzyAnimator != null)
                {
                    playerDizzyAnimator.speed = 0f;
                }
            }
        }

        public void IncreaseMaxHealth(float amount)
        {
            maxHealth += amount;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            CheckCollision(collision.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CheckCollision(other.gameObject);
        }

        private void CheckCollision(GameObject obj)
        {
            if (isDead)
                return;

            if (obj.CompareTag("Building") || obj.CompareTag("Body"))
            {
                Die();
            }
        }

        public void ResetPlayer()
        {
            isDead = false;

            currentHealth = 100f;

            if (playerDizzyAnimator != null)
            {
                playerDizzyAnimator.gameObject.SetActive(false);
            }

            if (playerMovement != null)
            {
                playerMovement.enabled = true;
                playerMovement.SetStamina(100f);

                Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.constraints = originalConstraints;
                }

                Animator anim = playerMovement.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.speed = 1f;
                }
            }
        }

        public override void TakeDamage(float amount)
        {
            if (GetComponentInChildren<PlayerMovement>().GetSprint() && vipSprint) return;

            base.TakeDamage(amount);
            SoundManager.Instance.PlaySound2D("Take_Damage");
            StreamChatManager.Instance.HandleStreamChat(StreamChatType.TAKE_DAMAGE, 5);

            foreach (var effect in visualEffects)
            {
                effect.PlayEffect();
            }
        }

        public void SafetyFirst()
        {
            collisionDmg = collisionDmg / 2;
        }

        public void VIPSprint()
        {
            vipSprint = true;
        }
    }
}