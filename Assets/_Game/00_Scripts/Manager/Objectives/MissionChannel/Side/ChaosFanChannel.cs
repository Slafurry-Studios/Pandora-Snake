using Game.Managerd;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/ChaosFanChannel")]
public class ChaosFanChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        GameManagerOld.Instance.AddSubs(200);
    }
}
