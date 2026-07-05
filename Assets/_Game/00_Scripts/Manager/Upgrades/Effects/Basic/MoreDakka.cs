using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "MoreDakka", menuName = "Game/Upgrade/Effect/Basic/MoreDakka")]
    public class MoreDakkaEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerShoot>().MoreDakka();
        }
    }
}