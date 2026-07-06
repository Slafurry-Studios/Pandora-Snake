using Game.Managerd;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/RoadDestroyerChannel")]
public class RoadDestroyerChannel : BaseObjectiveChannel 
{
    public override void OnCompleted()
    {
        GameManagerOld.Instance.AddSubs(200);
    }
}
