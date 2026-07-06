using Game.Core.Effects;
using Game.Generic;
using Slafurry.System.Audio;

namespace Game.Player
{
    public class PlayerHealth : Health
    {
        public TutorialManager tutorialManager;

        private IVisualEffect[] visualEffects;

        private PlayerMovement playerMovement;
        private PlayerDeath playerDeath;

        private bool vipSprint;
        private bool initialized;
        public void Initialize(PlayerMovement playerMovement, PlayerDeath playerDeath, float healthValue, IVisualEffect[] visualEffects)
        {
            this.playerMovement = playerMovement;
            this.playerDeath = playerDeath;

            this.visualEffects = visualEffects;

            SetMaxHealth(healthValue);
            initialized = true;
        }

        protected override void Die()
        {
            base.Die();
            playerDeath.Death();
        }
        public override void TakeDamage(float amount)
        {
            if (!initialized) return;

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