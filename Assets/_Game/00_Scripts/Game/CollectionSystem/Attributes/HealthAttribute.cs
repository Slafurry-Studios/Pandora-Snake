using UnityEngine;
using Game.Player;
using Slafurry.System.Audio;

namespace Game.Gameplay.Attributes
{
    [CreateAssetMenu(fileName = "NewHealthAttribute", menuName = "Game/Dropable/Attributes/Health Attribute")]
    public class HealthAttribute : AttributeData
    {
        public override void Apply(GameObject target, float amount)
        {
            PlayerHealth health = target.GetComponentInChildren<PlayerHealth>();
            if (health != null)
            {
                health.Heal(amount);
                Audio.PlaySFX2D("Collection", "Collection_Point");
            }
        }
    }
}
