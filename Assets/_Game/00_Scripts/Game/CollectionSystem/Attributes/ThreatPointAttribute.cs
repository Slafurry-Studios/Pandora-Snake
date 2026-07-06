using UnityEngine;
using Game.Managerd;
using System;
using Slafurry.System.Audio;

namespace Game.Gameplay.Attributes
{
    [CreateAssetMenu(fileName = "NewThreatPointAttribute", menuName = "Game/Dropable/Attributes/Threat Point Attribute")]
    public class ThreatPointAttribute : AttributeData
    {
        public override void Apply(GameObject target, float amount)
        {
            GameManagerOld.Instance.AddThreat((int)Math.Round(amount));
            Audio.PlaySFX2D("Collection", "Collection_Point");
        }
    }
}
