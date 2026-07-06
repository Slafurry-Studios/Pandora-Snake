using Game.Core.Effects;
using Game.Generic;
using Slafurry.System.Audio;
using UnityEngine;


namespace Game.Entities
{
    public class EntityHealth : Health
    {
        [Header("Objective")]
        [SerializeField] private BaseObjectiveChannel[] destroyChannel;
        [SerializeField] private ObjectiveScriptableObject objective;

        [Header("Death")]
        [SerializeField] private LayerMask deathLayerMask;
        [SerializeField] private StreamChatType deathChatType = StreamChatType.KILL_HOSTILES;

        [Header("Animation")]
        [SerializeField] private string dieAnim;
        [SerializeField] private string hitAnim;

        [Header("Audio")]
        [SerializeField] private string sfxCategory;
        [SerializeField] private string hitSound;
        [SerializeField] private string deathSound;

        protected IVisualEffect[] visualEffects;
        private EntityBrain entityBrain;
        private Collider2D entityCollider;

        public void Initialize(float health, EntityBrain entityBrain, Collider2D entityCollider, IVisualEffect[] visualEffects)
        {
            this.entityBrain = entityBrain;
            this.entityCollider = entityCollider;
            this.visualEffects = visualEffects;

            SetMaxHealth(health);
        }

        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);

            if (!isDead && entityBrain != null && entityBrain.Animator != null && !string.IsNullOrEmpty(hitAnim))
            {
                entityBrain.Animator.SetTrigger(hitAnim);
                Audio.PlaySFX2D(sfxCategory, hitSound);

                foreach (var effect in visualEffects)
                {
                    effect.PlayEffect();
                }
            }
        }

        protected override void Die()
        {
            base.Die();
            Audio.PlaySFX2D(sfxCategory, deathSound);

            SetLayerRecursively(gameObject, LayerMaskToLayer(deathLayerMask));

            foreach (BaseObjectiveChannel channel in destroyChannel)
            {
                channel.Raise(1);
            }

            if (objective != null)
                ObjectiveManager.Instance.AddObjective(objective.Objective);
            StreamChatManager.Instance.HandleStreamChat(deathChatType, 5);
            if (entityBrain != null)
            {
                if (entityBrain.Animator != null && !string.IsNullOrEmpty(dieAnim))
                {
                    entityBrain.Animator.Play(dieAnim, 0, 0f);
                }

                if (entityBrain.EntityMovement != null)
                {
                    entityBrain.EntityMovement.SetMovement(Vector2.zero, 0f);
                    ((Behaviour)entityBrain.EntityMovement).enabled = false;
                }

                entityBrain.enabled = false;
                entityCollider.isTrigger = true;
            }
        }

        private static int LayerMaskToLayer(LayerMask mask)
        {
            int value = mask.value;
            int layer = 0;
            while (value > 1)
            {
                value >>= 1;
                layer++;
            }
            return layer;
        }

        private static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }
}