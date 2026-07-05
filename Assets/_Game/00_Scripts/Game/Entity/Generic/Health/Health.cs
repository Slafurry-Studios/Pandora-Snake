using System;
using UnityEngine;
using Game.Core;

namespace Game.Generic
{
    public class Health : MonoBehaviour, IDamageable
    {
        public event Action<float> OnHealthChanged;
        public event Action<float> OnDamaged;
        public event Action<float> OnHealed;
        public event Action OnDied;

        [Header("Health Settings")]
        [SerializeField] protected float maxHealth = 10f;

        protected float currentHealth;
        protected bool isDead;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => isDead;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        protected virtual void Start()
        {
            OnHealthChanged?.Invoke(GetHealth());
        }

        public virtual void TakeDamage(float amount)
        {
            if (isDead || amount <= 0f)
                return;

            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            OnDamaged?.Invoke(amount);
            OnHealthChanged?.Invoke(GetHealth());

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        public virtual void Heal(float amount)
        {
            if (isDead || amount <= 0f)
                return;

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            OnHealed?.Invoke(amount);
            OnHealthChanged?.Invoke(GetHealth());
        }

        public virtual void SetHealth(float value)
        {
            currentHealth = Mathf.Clamp(value, 0f, maxHealth);

            OnHealthChanged?.Invoke(GetHealth());

            if (currentHealth <= 0f && !isDead)
            {
                Die();
            }
        }

        public float GetHealth()
        {
            return currentHealth;
        }

        protected virtual void Die()
        {
            if (isDead)
                return;

            isDead = true;

            OnDied?.Invoke();

            // Debug.Log($"{gameObject.name} died.");
        }
    }
}