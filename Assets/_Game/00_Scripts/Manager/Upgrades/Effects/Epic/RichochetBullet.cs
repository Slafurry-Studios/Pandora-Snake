using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "RichochetBullet", menuName = "Game/Upgrade/Effect/Epic/RichochetBullet")]
    public class RichochetBulletEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerShoot>().RichochetBullet();
        }
    }
}