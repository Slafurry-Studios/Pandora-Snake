using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "ThickSkin", menuName = "Game/Upgrade/Effect/Basic/ThickSkin")]
    public class ThickSkinEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
            playerHealth.IncreaseMaxHealth(1);
            playerHealth.Heal(100);
        }
    }
}