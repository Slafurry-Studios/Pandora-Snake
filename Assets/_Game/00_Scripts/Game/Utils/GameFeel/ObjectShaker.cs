using UnityEngine;

namespace Game.Core.Effects
{
    public class ObjectShaker : MonoBehaviour, IVisualEffect
    {
        [Header("Jitter Settings")]
        [Tooltip("How long the jitter lasts in seconds.")]
        [SerializeField] private float duration = 0.2f;

        [Tooltip("The magnitude of the jitter shake.")]
        [SerializeField] private float intensity = 0.2f;

        [Tooltip("If true, the jitter smoothly reduces its intensity to 0 over its duration.")]
        [SerializeField] private bool dampening = true;

        [Header("Axis Settings")]
        [SerializeField] private bool shakeX = true;
        [SerializeField] private bool shakeY = true;
        [SerializeField] private bool shakeZ = false;

        private float currentDuration;
        private float currentIntensity;
        private bool isShaking = false;
        
        private Vector3 currentPosOffset = Vector3.zero;

        public void PlayEffect()
        {
            currentDuration = duration;
            currentIntensity = intensity;
            isShaking = true;
        }
        public void PlayEffect(float customDuration, float customIntensity)
        {
            duration = customDuration;
            intensity = customIntensity;
            PlayEffect();
        }

        public void StopEffect()
        {
            if (isShaking)
            {
                isShaking = false;
                transform.localPosition -= currentPosOffset;
                currentPosOffset = Vector3.zero;
            }
        }

        private void LateUpdate()
        {
            if (!isShaking) return;

            if (currentDuration > 0)
            {
                float currentMag = currentIntensity;

                if (dampening)
                {
                    float normalizedTime = currentDuration / duration;
                    currentMag = Mathf.Lerp(0, currentIntensity, normalizedTime);
                }

                Vector3 randPoint = Random.insideUnitSphere * currentMag;
                float px = shakeX ? randPoint.x : 0f;
                float py = shakeY ? randPoint.y : 0f;
                float pz = shakeZ ? randPoint.z : 0f;
                
                Vector3 newPosOffset = new Vector3(px, py, pz);

                transform.localPosition = (transform.localPosition - currentPosOffset) + newPosOffset;
                currentPosOffset = newPosOffset;

                currentDuration -= Time.deltaTime;
            }
            else
            {
                StopEffect();
            }
        }
    }
}
