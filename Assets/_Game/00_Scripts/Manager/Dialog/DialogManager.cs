using Game.UI.HUD;
using UnityEngine;
using System;

namespace Game.Dialog
{
    public class DialogManager : MonoBehaviour
    {
        [SerializeField] private DialogBucket[] dialogBuckets;
        public Action<bool> OnDialogHUDReady;
        private DialogHUD dialogHUD;
        private DialogBucket currentBucket;
        private int currentIndex;

        private void Start()
        {
            dialogHUD = FindAnyObjectByType<DialogHUD>();

            if (dialogHUD != null)
            {
                OnDialogHUDReady?.Invoke(true);
            }
        }

        public void StartDialog(string id)
        {
            currentIndex = 0;
            currentBucket = null;

            foreach (DialogBucket bucket in dialogBuckets)
            {
                if (bucket.id == id)
                {
                    currentBucket = bucket;
                    break;
                }
            }

            if (currentBucket == null)
            {
                Debug.LogWarning($"Dialog with id '{id}' not found.");
                return;
            }

            dialogHUD.Show();
            ShowCurrentDialog();
        }

        public void NextDialog()
        {
            if (currentBucket == null)
                return;

            if (IsLast())
                return;

            currentIndex++;
            ShowCurrentDialog();
        }

        private void ShowCurrentDialog()
        {
            Dialog dialog = currentBucket.dialogs[currentIndex];

            dialogHUD.SetDialog(
                dialog.faceShotSprite,
                dialog.name,
                dialog.dialog,
                IsLast()
            );
        }

        private bool IsLast()
        {
            return currentIndex >= currentBucket.dialogs.Length - 1;
        }
    }
}