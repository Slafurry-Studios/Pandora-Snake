using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "FrameRateKiller", menuName = "Game/Upgrade/Effect/Rare/FrameRateKiller")]
    public class FrameRateKillerEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerShoot>().FrameRateKiller();
        }
    }
}