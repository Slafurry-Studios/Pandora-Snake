using System.Collections;
using Game.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.HUD
{
    public class PlayerSnakeFace : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image SnakeFaceUI;

        [Header("Assets")]
        [SerializeField] private Sprite SnakeHurt;
        [SerializeField] private Sprite SnakeNormal;

        [Header("Shake Settings")]
        [SerializeField] private float shakeDuration = 0.3f;
        [SerializeField] private float shakeIntensity = 10f;
        [SerializeField] private float hurtDisplayTime = 0.5f;

        private PlayerHealth playerHealth;
        private RectTransform rectTransform;
        private Vector3 originalPosition;
        private Coroutine hurtRoutine;

        void Start()
        {
            playerHealth = FindAnyObjectByType<PlayerHealth>();
            rectTransform = SnakeFaceUI.GetComponent<RectTransform>();
            originalPosition = rectTransform.localPosition;

            if (playerHealth != null) playerHealth.OnDamaged += TriggerHurt;
        }

        public void TriggerHurt(float damage)
        {
            if (hurtRoutine != null)
                StopCoroutine(hurtRoutine);

            hurtRoutine = StartCoroutine(HurtRoutine(damage));
        }

        private IEnumerator HurtRoutine(float damage)
        {
            SnakeFaceUI.sprite = SnakeHurt;

            float elapsed = 0f;
            while (elapsed < shakeDuration + damage / 5)
            {
                float offsetX = Random.Range(-1f, 1f) * shakeIntensity;
                float offsetY = Random.Range(-1f, 1f) * shakeIntensity;
                rectTransform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

                elapsed += Time.deltaTime;
                yield return null;
            }

            rectTransform.localPosition = originalPosition;

            if (hurtDisplayTime > shakeDuration)
                yield return new WaitForSeconds(hurtDisplayTime - shakeDuration);

            ReturnToNormal();
            hurtRoutine = null;
        }

        private void ReturnToNormal()
        {
            SnakeFaceUI.sprite = SnakeNormal;
        }
    }
}