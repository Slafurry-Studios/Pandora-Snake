using UnityEngine;
using Game.Upgrade;
using Game.Player;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "VIPSprint", menuName = "Game/Upgrade/Effect/Rare/VIPSprint")]
    public class VIPSprintEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<PlayerHealth>().VIPSprint();
        }
    }
}