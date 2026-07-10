using System.Collections.Generic;
using Game.Core.Effects;
using Game.Player;
using UnityEngine;

namespace Game.Entities
{
    public class Car : MonoBehaviour
    {
        [SerializeField] private CarData carData;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private List<EntityState> entityStates;

        private CarHealth carHealth;
        private EntityBrain carBrain;
        private IEntityMovement carMovement;
        private IEntityShoot carShoot;
        private IVisualEffect[] visualEffects;
        private Collider2D carCollider;
        private Rigidbody2D rb;
        private bool initialized;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            carCollider = GetComponent<Collider2D>();

            carHealth = GetComponentInChildren<CarHealth>();
            carBrain = GetComponentInChildren<EntityBrain>();

            carMovement = GetComponentInChildren<IEntityMovement>();
            carShoot = GetComponentInChildren<IEntityShoot>();

            visualEffects = GetComponentsInChildren<IVisualEffect>();

            foreach (var state in entityStates)
            {
                state.Initialize(carData);
            }

            carHealth.Initialize(carData.Health, carData.DeathChatType, carBrain, carCollider, visualEffects);
            carBrain.Initialize(entityStates, carMovement, carShoot, animator);

            carMovement.Initialize(carBrain, carData, spriteRenderer, rb);

            carShoot?.Initialize(carBrain, carData.BulletFireData, animator);
            carShoot?.InitKeys(carData.AudioCategory, carData.ShootSFX);
            initialized = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!initialized) return;
            carHealth.CheckCollision(collision.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!initialized) return;
            carHealth.CheckCollision(other.gameObject);
        }
    }
}