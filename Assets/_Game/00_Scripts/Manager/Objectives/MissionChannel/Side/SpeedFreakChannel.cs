using Game.Managerd;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/SpeedFreakChannel")]
public class SpeedFreakChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManagerOld.Instance.AddSubs(200);
    }
}
