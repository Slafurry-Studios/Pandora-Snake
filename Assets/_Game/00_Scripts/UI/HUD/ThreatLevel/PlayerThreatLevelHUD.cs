using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game.Manager;

namespace Game.UI.HUD
{
    public class PlayerThreatLevelHUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider threatSlider;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Sprite[] threatBackgrounds;
        [SerializeField] private int[] threatThresholds;

        [Header("Shake Settings")]
        [SerializeField] private RectTransform sliderRectTransform;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private int debugState = 0;
        [SerializeField][Range(0f, 1f)] private float debugThreatValue = 0f;

        private readonly float[] shakeAmplitudes = { 0f, 2f, 3.5f, 5f };
        private readonly float[] shakeSpeeds = { 0f, 0.06f, 0.04f, 0.025f };

        private ThreatPointManager threatPointManager;
        private Coroutine shakeCoroutine;
        private int currentState = -1;
        private int lastDebugState = -1;
        private float lastDebugThreatValue = -1f;
        private Vector2 originalPosition;

        void Start()
        {
            threatPointManager = FindAnyObjectByType<ThreatPointManager>();
            threatPointManager.OnCurrentThreatStateChanged += UpdateBackground;
            threatPointManager.OnThreatPointIncreased += UpdateThreat;

            originalPosition = sliderRectTransform.localPosition;
        }

        void OnDisable()
        {
            threatPointManager.OnCurrentThreatStateChanged -= UpdateBackground;
            threatPointManager.OnThreatPointIncreased -= UpdateThreat;
        }

        void Update()
        {
            if (!debugMode) return;

            debugState = Mathf.Clamp(debugState, 0, shakeAmplitudes.Length - 1);

            if (debugState != lastDebugState)
            {
                lastDebugState = debugState;
                UpdateBackground(debugState);
            }

            if (!Mathf.Approximately(debugThreatValue, lastDebugThreatValue))
            {
                lastDebugThreatValue = debugThreatValue;
                UpdateThreat(debugThreatValue, 1f);
            }
        }

        private void UpdateThreat(float currentThreat, float maxThreat)
        {
            threatSlider.value = currentThreat;
        }

        private void UpdateBackground(int state)
        {
            if (backgroundImage != null &&
                threatBackgrounds != null &&
                state < threatBackgrounds.Length)
            {
                backgroundImage.sprite = threatBackgrounds[state];
            }

            if (state == currentState) return;
            currentState = state;

            if (shakeCoroutine != null)
                StopCoroutine(shakeCoroutine);

            if (state <= 0 || shakeAmplitudes[state] <= 0f)
            {
                sliderRectTransform.localPosition = originalPosition;
                return;
            }

            shakeCoroutine = StartCoroutine(ShakeSlider(state));
        }

        private IEnumerator ShakeSlider(int state)
        {
            float amplitude = shakeAmplitudes[state];
            float speed = shakeSpeeds[state];
            float time = 0f;

            while (true)
            {
                time += Time.unscaledDeltaTime;

                float x = Mathf.Sin(time / speed * Mathf.PI * 2f) * amplitude;
                float y = Mathf.Sin(time / speed * Mathf.PI * 2.7f) * amplitude * 0.3f;

                sliderRectTransform.localPosition = (Vector3)originalPosition + new Vector3(x, y, 0f);
                yield return null;
            }
        }

        private void OnDestroy()
        {
            if (threatPointManager == null) return;
            threatPointManager.OnCurrentThreatStateChanged -= UpdateBackground;
            threatPointManager.OnThreatPointIncreased -= UpdateThreat;
        }
    }
}