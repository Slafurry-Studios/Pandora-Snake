using System.Collections;
using Game.Core.Effects;
using Slafurry.Core.Interface;
using Slafurry.System;
using UnityEngine;

namespace Game.Player
{
    public class Player : MonoBehaviour, IInitializable
    {
        [SerializeField] private PlayerData playerData;
        public PlayerData PlayerData => playerData;

        public Rigidbody2D RigidBody2D { get; private set; }

        public PlayerHealth PlayerHealth { get; private set; }
        public PlayerStamina PlayerStamina { get; private set; }

        public PlayerDeath PlayerDeath { get; private set; }
        public PlayerGrowth PlayerGrowth { get; private set; }

        public PlayerMovement PlayerMovement { get; private set; }
        public PlayerShoot PlayerShoot { get; private set; }

        public SnakeTailManager PlayerTail { get; private set; }
        public PlayerCollision PlayerCollision { get; private set; }
        public IVisualEffect[] visualEffects { get; private set; }

        public int Priority => 2;

        void Awake()
        {
            LoadingSystem.Instance.Register(this);
        }

        public IEnumerator Initialize()
        {
            yield return null;
        }

        public void PostInitialize()
        {
            RigidBody2D = GetComponentInChildren<Rigidbody2D>();

            PlayerHealth = GetComponentInChildren<PlayerHealth>();
            PlayerStamina = GetComponentInChildren<PlayerStamina>();

            PlayerGrowth = GetComponentInChildren<PlayerGrowth>();
            PlayerDeath = GetComponentInChildren<PlayerDeath>();

            PlayerMovement = GetComponentInChildren<PlayerMovement>();
            PlayerShoot = GetComponentInChildren<PlayerShoot>();

            PlayerCollision = GetComponentInChildren<PlayerCollision>();
            PlayerTail = GetComponentInChildren<SnakeTailManager>();

            visualEffects = GetComponentsInChildren<IVisualEffect>();

            PlayerHealth.Initialize(PlayerMovement, PlayerDeath, playerData.PlayerHealth, visualEffects);
            PlayerStamina.Initialize(playerData.PlayerMovementData);

            PlayerGrowth.Initialize(PlayerTail);
            PlayerDeath.Initialize(PlayerHealth, PlayerStamina, PlayerMovement, RigidBody2D);

            PlayerMovement.Initialize(PlayerStamina, RigidBody2D, playerData.PlayerMovementData);
            PlayerShoot.Initialize(playerData.CombatData);

            PlayerCollision.Initialize(PlayerHealth, PlayerDeath, playerData.CollisionDamage);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            PlayerCollision.CheckCollision(collision.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerCollision.CheckCollision(other.gameObject);
        }

    }
}