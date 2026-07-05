using UnityEngine;
using Game.Manager;
using System;
using Slafurry.System.Audio;

namespace Game.Gameplay.Attributes
{
    [CreateAssetMenu(fileName = "NewSubsAttribute", menuName = "Game/Dropable/Attributes/Subs Attribute")]
    public class SubsAttribute : AttributeData
    {
        public override void Apply(GameObject target, float amount)
        {
            GameManager.Instance.AddSubs((int)Math.Round(amount));
            Audio.PlaySFX2D("Collection", "Collection_Point");
        }
    }
}
