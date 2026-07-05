using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "AverageBulletEnjoyer", menuName = "Game/Upgrade/Effect/Rare/AverageBulletEnjoyer")]
    public class AverageBulletEnjoyerEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerShoot>().AverageBulletEnjoyer();
        }
    }
}