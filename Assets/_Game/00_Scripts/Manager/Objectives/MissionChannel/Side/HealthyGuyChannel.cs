using Game.Managerd;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/HealthyGuyChannel")]
public class HealthyGuyChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManagerOld.Instance.AddSubs(200);
    }
}
