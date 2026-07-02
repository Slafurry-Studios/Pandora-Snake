using Game.Manager;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/RoadDestroyerChannel")]
public class RoadDestroyerChannel : BaseObjectiveChannel 
{
    public override void OnCompleted()
    {
        GameManager.Instance.AddSubs(200);
    }
}
