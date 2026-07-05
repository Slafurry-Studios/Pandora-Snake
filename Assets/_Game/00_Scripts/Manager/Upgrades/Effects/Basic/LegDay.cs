using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "LegDay", menuName = "Game/Upgrade/Effect/Basic/LegDay")]
    public class LegDayEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerMovement>().LegDay();
        }
    }
}