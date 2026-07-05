using UnityEngine;

namespace Game.Dialog
{
    [CreateAssetMenu(fileName = "NewDialogBucket", menuName = "Game/Dialog/Bucket")]
    public class DialogBucket : ScriptableObject
    {
        public string id;
        public Dialog[] dialogs;
    }
}