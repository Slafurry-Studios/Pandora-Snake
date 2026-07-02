using UnityEngine;
using Game.Player;

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
                SoundManager.Instance.PlaySound2D("Collection_Point");
            }
        }
    }
}
