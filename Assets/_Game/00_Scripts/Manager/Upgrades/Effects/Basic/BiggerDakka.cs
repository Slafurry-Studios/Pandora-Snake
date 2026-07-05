using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "BiggerDakka", menuName = "Game/Upgrade/Effect/Basic/BiggerDakka")]
    public class BiggerDakkaEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerShoot>().BiggerDakka();
        }
    }
}