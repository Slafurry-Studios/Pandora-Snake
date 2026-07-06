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

        public void Initialize()
        {
            Player player = GetComponentInParent<Player>();
            visualEffects = player.GetComponentsInChildren<IVisualEffect>();
            playerMovement = player.PlayerMovement;
            playerDeath = player.PlayerDeath;

            SetMaxHealth(player.PlayerData.PlayerHealth);
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