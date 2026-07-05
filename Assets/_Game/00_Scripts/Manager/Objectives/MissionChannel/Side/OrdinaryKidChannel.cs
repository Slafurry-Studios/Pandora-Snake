using Game.Manager;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/OrdinaryKidChannel")]
public class OrdinaryKidChannel : BaseObjectiveChannel
{
    public override void OnCompleted()
    {
        Debug.Log("OrdinaryKidChannel completed");
        FindAnyObjectByType<IntroCutscene>().NextCutscene();
        FindAnyObjectByType<IntroCutscene>().NextCutscene();
    }
}
