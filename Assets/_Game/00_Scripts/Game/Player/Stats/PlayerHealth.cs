using Game.Core.Effects;
using Game.Generic;
using Slafurry.System.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Player
{
    public class PlayerHealth : Health
    {
        public IVisualEffect[] visualEffects;

        private bool vipSprint;
        public TutorialManager tutorialManager;
        private PlayerMovement playerMovement;
        private PlayerDeath playerDeath;
        public void Initialize()
        {
            visualEffects = GetComponentsInChildren<IVisualEffect>();

            Player player = GetComponentInParent<Player>();
            playerMovement = player.PlayerMovement;
            playerDeath = player.PlayerDeath;
        }

        protected override void Die()
        {
            base.Die();
            playerDeath.Death();
        }
        public override void TakeDamage(float amount)
        {
            if (playerMovement.GetSprint() && vipSprint) return;

            base.TakeDamage(amount);
            Audio.PlaySFX2D("Player", "Take_Damage");
            StreamChatManager.Instance.HandleStreamChat(StreamChatType.TAKE_DAMAGE, 5);

            foreach (var effect in visualEffects)
            {
                effect.PlayEffect();
            }

            tutorialManager.StartTutorial("health_tutorial");

        }

        public void IncreaseMaxHealth(float amount)
        {
            maxHealth += amount;
        }

        public void VIPSprint()
        {
            vipSprint = true;
        }
    }
}