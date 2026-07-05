using System;
using UnityEngine;

namespace Game.Gameplay
{
    [Serializable]
    public struct AttributeModifier
    {
        [Tooltip("The attribute to apply.")]
        public AttributeData attribute;

        [Tooltip("The amount to apply (e.g., healing amount, points to add).")]
        public float amount;
    }
}
