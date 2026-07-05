using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "ExplosiveAmmo", menuName = "Game/Upgrade/Effect/Epic/ExplosiveAmmo")]
    public class ExplosiveAmmoEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerShoot>().ExplosiveAmmo();
        }
    }
}