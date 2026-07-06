using System.Collections.Generic;
using Game.Core.Effects;
using Game.Entities;
using Game.Entity;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Entities
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private EntityData entityData;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private List<EntityState> entityStates;

        private EntityHealth entityHealth;
        private EntityBrain entityBrain;
        private IEntityMovement entityMovement;
        private IEntityShoot entityShoot;
        private IVisualEffect[] visualEffects;
        private Collider2D entityCollider;
        private Rigidbody2D rb;
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            entityCollider = GetComponent<Collider2D>();

            entityHealth = GetComponentInChildren<EntityHealth>();
            entityBrain = GetComponentInChildren<EntityBrain>();

            entityMovement = GetComponentInChildren<IEntityMovement>();
            entityShoot = GetComponentInChildren<IEntityShoot>();

            visualEffects = GetComponentsInChildren<IVisualEffect>();

            entityHealth.Initialize(entityData.Health, entityBrain, entityCollider, visualEffects);
            entityBrain.Initialize(entityStates, entityMovement, entityShoot, animator);

            entityMovement.Initialize(entityBrain, spriteRenderer, rb);

            entityShoot.Initialize(entityBrain, entityData.BulletFireData, animator);
            entityShoot.InitKeys(entityData.AudioCategory, entityData.ShootSFX);
        }
    }
}