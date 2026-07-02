using UnityEngine;
using Game.Generic;
using Game.Core.Effects;
using Game.Manager;


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
        GameManager.Instance.AddThreat(ThreatPointValue);
        GameManager.Instance.AddSubs(SubsPointValue);
        base.Die();
        SoundManager.Instance.PlaySound2D("Building_Destroyed");

        foreach (BaseObjectiveChannel channel in destroyChannel)
        {
            channel.Raise(1);
        }

        ObjectiveManager.Instance.AddObjective(destroyObjectives.Objective);

        Destroy(gameObject);
    }
}