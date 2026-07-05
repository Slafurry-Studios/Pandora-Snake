using UnityEngine;
using Game.Upgrade;

namespace Game.Manager
{

    public class GameManager : Singleton<GameManager>
    {
        public ThreatPointManager threatManager { get; private set; }
        public SubscriptionPointManager subsManager { get; private set; }
        public UpgradeManager upgradeManager { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            threatManager = FindAnyObjectByType<ThreatPointManager>();
            subsManager = FindAnyObjectByType<SubscriptionPointManager>();
            upgradeManager = FindAnyObjectByType<UpgradeManager>();
        }

        public void AddThreat(int amount)
        {
            threatManager.IncreasePoints(amount);
        }

        public void AddSubs(int amount)
        {
            subsManager.IncreasePoints(amount);
        }
    }
}