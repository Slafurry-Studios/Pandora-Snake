using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public class DirectAttributeGiver : MonoBehaviour
    {
        [Header("Attributes to Give")]
        [Tooltip("The attributes applied directly to the target when triggered (e.g. on NPC death).")]
        public List<AttributeModifier> attributes = new List<AttributeModifier>();

        public void GiveAttributesToPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                GiveAttributes(player);
            }
            else
            {
                Debug.LogWarning("DirectAttributeGiver: Player not found!");
            }
        }

        public void GiveAttributes(GameObject target)
        {
            if (target == null) return;

            foreach (var modifier in attributes)
            {
                if (modifier.attribute != null)
                {
                    modifier.attribute.Apply(target, modifier.amount);
                }
            }
        }
    }
}
