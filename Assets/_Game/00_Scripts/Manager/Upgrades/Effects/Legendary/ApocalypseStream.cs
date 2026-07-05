using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "ApocalypseStream", menuName = "Game/Upgrade/Effect/Legendary/ApocalypseStream")]
    public class ApocalypseStreamEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerShoot>().ApocalypseStream();
        }
    }
}