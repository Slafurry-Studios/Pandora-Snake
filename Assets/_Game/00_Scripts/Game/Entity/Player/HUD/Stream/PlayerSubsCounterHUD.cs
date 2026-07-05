using UnityEngine;
using TMPro;
using Game.Manager;

namespace Game.UI.HUD
{

    public class PlayerSubsCounterHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI subsText;
        private SubscriptionPointManager subsPointManager;

        void Start()
        {
            subsPointManager = FindAnyObjectByType<SubscriptionPointManager>();
            subsPointManager.OnSubsPointIncreased += UpdateSubscriber;
        }

        public void UpdateSubscriber(int amount)
        {
            subsText.text = "Subs: " + amount;
        }
    }

}