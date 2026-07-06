using UnityEngine;
using Game.Generic;
using Game.Core.Effects;
using Game.Managerd;
using Game.Gameplay;
using Slafurry.System.Audio;


public class BuildingHealth : Health
{
    [SerializeField] private int ThreatPointValue;
    [SerializeField] private int SubsPointValue;
    [SerializeField] private ObjectiveScriptableObject destroyObjectives;

    private IVisualEffect[] effects;


    [SerializeField] private BaseObjectiveChannel[] destroyChannel;

    protected override void Start()
    {
        base.Start();
        effects = GetComponentsInChildren<IVisualEffect>();
    }
    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        foreach (IVisualEffect effect in effects)
        {
            effect.PlayEffect();
        }
    }

    protected override void Die()
    {
        StreamChatManager.Instance.HandleStreamChat(StreamChatType.DESTROY_BUILDING, 3);
        GameManagerOld.Instance.AddThreat(ThreatPointValue);
        GameManagerOld.Instance.AddSubs(SubsPointValue);
        base.Die();
        Audio.PlaySFX2D("Building", "Destroyed");

        foreach (BaseObjectiveChannel channel in destroyChannel)
        {
            channel.Raise(1);
        }

        ObjectiveManager.Instance.AddObjective(destroyObjectives.Objective);
        GetComponent<LootDropper>()?.DropLoot();
        Destroy(gameObject);
    }
}