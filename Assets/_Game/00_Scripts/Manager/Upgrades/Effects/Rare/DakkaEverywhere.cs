using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "DakkaEverywhere", menuName = "Game/Upgrade/Effect/Rare/DakkaEverywhere")]
    public class DakkaEverywhereEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerShoot>().DakkaEverywhere();
        }
    }
}