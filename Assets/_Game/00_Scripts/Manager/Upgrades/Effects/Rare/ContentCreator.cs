using UnityEngine;
using Game.Upgrade;
using Game.Manager;

namespace Game.Upgrade.Effect
{
    [CreateAssetMenu(fileName = "ContentCreator", menuName = "Game/Upgrade/Effect/Rare/ContentCreator")]
    public class ContentCreatorEffect : UpgradeCardEffect
    {
        public override void Apply()
        {
            FindAnyObjectByType<SubscriptionPointManager>().ContentCreator();
        }
    }
}