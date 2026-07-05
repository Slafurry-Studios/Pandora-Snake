using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "InfiniteCardio", menuName = "Game/Upgrade/Effect/Basic/InfiniteCardio")]
    public class InfiniteCardioEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerMovement>().InfiniteCardio();
        }
    }
}