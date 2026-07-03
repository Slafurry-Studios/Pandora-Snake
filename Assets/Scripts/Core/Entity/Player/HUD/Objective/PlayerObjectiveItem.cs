using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.UI.HUD
{
    public class PlayerObjectiveItem : MonoBehaviour
    {
        [SerializeField] private Sprite objectiveCompleted;
        [SerializeField] private Sprite objectiveUncompleted;
        [SerializeField] private Image checkboxImage;
        [SerializeField] private TextMeshProUGUI objectiveText;

        [Header("Completion Fade")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.6f;
        public event System.Action<PlayerObjectiveItem> OnFadeCompleted;

        private ObjectiveProgress progress;
        private Coroutine fadeRoutine;

        public bool IsCompleted { get; private set; }

        private void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public void Setup(ObjectiveProgress data)
        {
            progress = data;
            IsCompleted = false;
            canvasGroup.alpha = 1f;
            checkboxImage.sprite = objectiveUncompleted;
            RefreshText();
        }


        public void TaskCompleted()
        {
            if (IsCompleted) return;

            IsCompleted = true;
            checkboxImage.sprite = objectiveCompleted;
            RefreshText();

            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeToCompletedState());
        }

        private System.Collections.IEnumerator FadeToCompletedState()
        {
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            OnFadeCompleted?.Invoke(this);
        }

        public void UpdateStatus(string stat)
        {
            objectiveText.text = stat;
        }

        public void RefreshText()
        {
            if (progress == null) return;

            Transform nearest = ObjectiveTargetRegistry.GetNearest(
                progress.Data.Channel,
                Camera.main.transform.position
            );

            // string nearestInfo = nearest != null
            //     ? $" | Nearest: {nearest.name} ({Vector3.Distance(Camera.main.transform.position, nearest.position):F0}m)"
            //     : " | No target found";

            string nearestInfo = nearest != null
                ? $" | ({Vector3.Distance(Camera.main.transform.position, nearest.position):F0}m)"
                : "";

            objectiveText.text = $"{progress.Data.DisplayName} ({progress.CurrentValue}/{progress.Data.ObjectiveThreshold}){nearestInfo}";
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
        }

        public ObjectiveProgress GetProgress() => progress;
    }
}