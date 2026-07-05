using System.Collections;
using UnityEngine;

namespace Game.UI.HUD
{
    public class UISlideIn : MonoBehaviour
    {
        public enum Direction { Top, Bottom, Left, Right }

        [Header("Reference")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform canvasRect;

        [Header("Direction")]
        [SerializeField] private Direction fromDirection = Direction.Left;
        [SerializeField] private float extraOffset = 100f;

        [Header("Timing")]
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float delay = 0f;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Options")]
        [SerializeField] private bool playOnEnable = true;
        [SerializeField] private bool useUnscaledTime = true;

        private Vector2 targetPos;
        private Vector2 startPos;
        private Coroutine routine;

        private void Awake()
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            if (canvasRect == null)
            {
                Canvas canvas = GetComponentInParent<Canvas>();
                if (canvas != null)
                    canvasRect = canvas.rootCanvas.transform as RectTransform;
            }

            targetPos = rectTransform.anchoredPosition;
        }

        private void OnEnable()
        {
            if (playOnEnable)
                Play();
        }

        public void Play()
        {
            if (routine != null)
                StopCoroutine(routine);

            startPos = CalculateStartPos();
            rectTransform.anchoredPosition = startPos;

            routine = StartCoroutine(SlideRoutine());
        }

        private Vector2 CalculateStartPos()
        {
            float ownWidth = rectTransform.rect.width;
            float ownHeight = rectTransform.rect.height;

            float screenWidth = canvasRect != null ? canvasRect.rect.width : Screen.width;
            float screenHeight = canvasRect != null ? canvasRect.rect.height : Screen.height;

            float horizontalDistance = (screenWidth * 0.5f) + (ownWidth * 0.5f) + extraOffset;
            float verticalDistance = (screenHeight * 0.5f) + (ownHeight * 0.5f) + extraOffset;

            switch (fromDirection)
            {
                case Direction.Top:
                    return targetPos + new Vector2(0f, verticalDistance);
                case Direction.Bottom:
                    return targetPos - new Vector2(0f, verticalDistance);
                case Direction.Left:
                    return targetPos - new Vector2(horizontalDistance, 0f);
                case Direction.Right:
                    return targetPos + new Vector2(horizontalDistance, 0f);
                default:
                    return targetPos;
            }
        }

        private IEnumerator SlideRoutine()
        {
            if (delay > 0f)
                yield return useUnscaledTime ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay);

            float t = 0f;
            while (t < duration)
            {
                t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float normalized = Mathf.Clamp01(t / duration);
                float eased = easeCurve.Evaluate(normalized);
                rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, eased);
                yield return null;
            }

            rectTransform.anchoredPosition = targetPos;
        }
    }
}