using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "SafetyFirst", menuName = "Game/Upgrade/Effect/Rare/SafetyFirst")]
    public class SafetyFirstEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerHealth>().SafetyFirst();
        }
    }
}