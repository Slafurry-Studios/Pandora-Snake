using UnityEngine;

namespace Game.Gameplay
{
    public abstract class AttributeData : ScriptableObject
    {
        public abstract void Apply(GameObject target, float amount);
    }
}
