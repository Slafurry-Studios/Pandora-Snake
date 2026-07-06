using UnityEngine;
using Game.Entities;

namespace Game.Entities.Boss
{
    /// <summary>
    /// Specialized EntityBrain controller for Boss entities.
    /// Automatically enforces requiring a BossHealth component.
    /// Configure individual state behaviors directly on the attached state scripts (BossAttackState, BossChaseState, BossLandState).
    /// </summary>
    [RequireComponent(typeof(BossHealth))]
    public class BossBrain : EntityBrain
    {
    }
}