using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "MoreEspresso", menuName = "Game/Upgrade/Effect/Basic/MoreEspresso")]
    public class MoreEspressoEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerShoot>().MoreEspresso();
        }
    }
}