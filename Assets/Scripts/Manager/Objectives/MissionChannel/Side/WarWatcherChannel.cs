using Game.Manager;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/WarWatcherChannel")]
public class WarWatcherChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManager.Instance.AddSubs(500);
    }
}
