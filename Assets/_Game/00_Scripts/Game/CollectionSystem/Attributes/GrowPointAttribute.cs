using UnityEngine;
using Game.Player;
using Slafurry.System.Audio;

namespace Game.Gameplay.Attributes
{
    [CreateAssetMenu(fileName = "NewGrowPointAttribute", menuName = "Game/Dropable/Attributes/Grow Point Attribute")]
    public class GrowPointAttribute : AttributeData
    {
        public override void Apply(GameObject target, float amount)
        {
            PlayerGrowth growth = target.GetComponentInChildren<PlayerGrowth>();
            if (growth != null)
            {
                growth.AddGrowPoints(Mathf.RoundToInt(amount));
                Audio.PlaySFX2D("Collection", "Collection_Point");
            }
        }
    }
}
