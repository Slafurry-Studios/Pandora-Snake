using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Dialog;
using System;

namespace Game.UI.HUD
{
    public class DialogHUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image faceShot;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] private GameObject nextDialogClue;
        [SerializeField] private GameObject dialogUIPrefab;

        [Header("Type Effect")]
        [SerializeField] private float typingSpeed = 0.03f;

        [Header("SFX")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip typeLoopSfx;

        [Header("Options")]
        [SerializeField] private bool allowSkip = true;

        private DialogManager dialogManager;

        private bool isLast;
        private bool isTyping;

        private string currentDialog;
        private Coroutine typingCoroutine;
        public Action OnDialogEnd;

        private void Start()
        {
            dialogManager = FindAnyObjectByType<DialogManager>();
        }

        private void Update()
        {
            if (!allowSkip) return;
            if (!dialogUIPrefab.activeSelf) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                SkipDialog();
            }
        }

        public void SkipDialog()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            StopTypeSfx();
            isTyping = false;
            isLast = false;

            Hide();
        }

        public void Show()
        {
            Time.timeScale = 0f;
            dialogUIPrefab.SetActive(true);
            PlayerManager.Instance.Pause();
        }

        public void Hide()
        {
            Time.timeScale = 1f;
            PlayerManager.Instance.Resume();
            OnDialogEnd?.Invoke();
            dialogUIPrefab.SetActive(false);
        }

        public void SetDialog(Sprite faceShotSprite, string characterName, string dialog, bool isLast)
        {
            faceShot.sprite = faceShotSprite;
            nameText.text = characterName;

            currentDialog = dialog;
            this.isLast = isLast;

            nextDialogClue.SetActive(false);

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeDialog());
        }

        private IEnumerator TypeDialog()
        {
            isTyping = true;
            dialogText.text = "";

            PlayTypeSfx();

            foreach (char c in currentDialog)
            {
                dialogText.text += c;
                yield return new WaitForSecondsRealtime(typingSpeed);
            }

            StopTypeSfx();

            dialogText.text = currentDialog;
            isTyping = false;

            if (!isLast)
                nextDialogClue.SetActive(true);
        }

        public void NextDialog()
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                StopTypeSfx();

                dialogText.text = currentDialog;
                isTyping = false;

                if (!isLast)
                    nextDialogClue.SetActive(true);

                return;
            }

            if (isLast)
            {
                Hide();
                isLast = false;
                return;
            }

            dialogManager.NextDialog();
        }

        private void PlayTypeSfx()
        {
            if (audioSource == null || typeLoopSfx == null)
                return;

            audioSource.clip = typeLoopSfx;
            audioSource.loop = true;
            audioSource.Play();
        }

        private void StopTypeSfx()
        {
            if (audioSource == null)
                return;

            audioSource.Stop();
        }
    }
}