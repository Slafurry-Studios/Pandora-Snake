using Game.Manager;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/HealthyGuyChannel")]
public class HealthyGuyChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManager.Instance.AddSubs(200);
    }
}
