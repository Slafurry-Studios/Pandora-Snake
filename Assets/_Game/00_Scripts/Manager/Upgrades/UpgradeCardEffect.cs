using UnityEngine;

namespace Game.Upgrade
{
    public abstract class UpgradeCardEffect : ScriptableObject
    {
        public abstract void Apply();
    }
}