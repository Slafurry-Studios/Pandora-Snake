using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Slafurry.System.Audio;

namespace Game.UI.HUD
{
    public class PlayerDonationHUD : MonoBehaviour
    {
        [SerializeField] private GameObject donationUI;
        [SerializeField] private Image donationImage;
        [SerializeField] private RectTransform donationRect;
        [SerializeField] private CanvasGroup donationCanvasGroup;

        [Header("Settings")]
        [SerializeField] private float showDuration = 2.5f;
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.4f;
        [SerializeField] private float slideDistance = 30f;

        private Coroutine currentRoutine;
        private Vector2 originalPos;

        protected void Awake()
        {
            if (donationRect == null) donationRect = donationUI.GetComponent<RectTransform>();
            if (donationCanvasGroup == null) donationCanvasGroup = donationUI.GetComponent<CanvasGroup>();
            originalPos = donationRect.anchoredPosition;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => ObjectiveManager.Instance != null);
            ObjectiveManager.Instance.OnObjectiveAdded += HandleObjectiveAdded;
        }

        private void OnDisable()
        {
            ObjectiveManager.Instance.OnObjectiveAdded -= HandleObjectiveAdded;
        }

        private void HandleObjectiveAdded(Objective objective)
        {
            if (objective.Channel != null && objective.Channel.useDonation)
            {
                UpdateDonation(objective.Channel.donationSprite);
            }
        }

        public void UpdateDonation(Sprite donationSprite)
        {
            donationImage.sprite = donationSprite;

            if (currentRoutine != null)
                StopCoroutine(currentRoutine);

            currentRoutine = StartCoroutine(ShowDonationRoutine());
        }

        private IEnumerator ShowDonationRoutine()
        {
            donationUI.SetActive(true);
            Audio.PlaySFX2D("UI", "Superchat");
            Vector2 startPos = originalPos - new Vector2(0f, slideDistance);
            donationRect.anchoredPosition = startPos;
            donationCanvasGroup.alpha = 0f;

            float t = 0f;
            while (t < fadeInDuration)
            {
                t += Time.unscaledDeltaTime;
                float p = Mathf.Clamp01(t / fadeInDuration);
                float eased = 1f - Mathf.Pow(1f - p, 3f);

                donationCanvasGroup.alpha = eased;
                donationRect.anchoredPosition = Vector2.Lerp(startPos, originalPos, eased);

                yield return null;
            }

            donationCanvasGroup.alpha = 1f;
            donationRect.anchoredPosition = originalPos;

            yield return new WaitForSecondsRealtime(showDuration);

            t = 0f;
            while (t < fadeOutDuration)
            {
                t += Time.unscaledDeltaTime;
                float p = Mathf.Clamp01(t / fadeOutDuration);
                donationCanvasGroup.alpha = 1f - p;
                yield return null;
            }

            donationCanvasGroup.alpha = 0f;
            donationUI.SetActive(false);

            currentRoutine = null;
        }
    }
}