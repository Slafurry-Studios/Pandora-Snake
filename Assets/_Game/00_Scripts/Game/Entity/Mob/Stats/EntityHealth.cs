using Game.AI;
using Game.Core.Effects;
using Game.Generic;
using Slafurry.System.Audio;
using UnityEngine;

public class EntityHealth : Health
{
    [SerializeField] private BaseObjectiveChannel[] destroyChannel;
    [SerializeField] private ObjectiveScriptableObject objective;

    private EntityBrain entityBrain;
    [SerializeField] private string dieAnim;
    [SerializeField] private string hitAnim;
    [SerializeField] private string hitSound;
    [SerializeField] private string deathSound;
    [SerializeField] private LayerMask deathLayerMask;
    [SerializeField] private StreamChatType deathChatType = StreamChatType.KILL_HOSTILES;
    protected IVisualEffect[] visualEffects;

    protected override void Awake()
    {
        base.Awake();
        entityBrain = GetComponent<EntityBrain>();
        visualEffects = GetComponentsInChildren<IVisualEffect>();
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        if (!isDead && entityBrain != null && entityBrain.aiAnimation != null && !string.IsNullOrEmpty(hitAnim))
        {
            entityBrain.aiAnimation.SetTrigger(hitAnim);
            Audio.PlaySFX2D("Player", hitSound);

            foreach (var effect in visualEffects)
            {
                effect.PlayEffect();
            }
        }
    }

    protected override void Die()
    {
        base.Die();
        Audio.PlaySFX2D("Player", deathSound);

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
            if (entityBrain.aiAnimation != null && !string.IsNullOrEmpty(dieAnim))
            {
                entityBrain.aiAnimation.Play(dieAnim, 0, 0f);
            }

            if (entityBrain.Movement != null)
            {
                entityBrain.Movement.SetMovement(Vector2.zero, 0f);
                ((Behaviour)entityBrain.Movement).enabled = false;
            }

            entityBrain.enabled = false;
            GetComponent<Collider2D>().isTrigger = true;
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