using Game.Managerd;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/WarWatcherChannel")]
public class WarWatcherChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManagerOld.Instance.AddSubs(500);
    }
}
