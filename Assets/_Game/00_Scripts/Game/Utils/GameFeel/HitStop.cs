using System.Collections;
using UnityEngine;
using Game.Core;

namespace Game.Core.Effects
{
    public class HitStop : MonoBehaviour, IVisualEffect
    {
        private bool isHitStopping;
        private float defaultDuration = 0.1f;

        public void Stop(float duration)
        {
            if (isHitStopping) return;
            StartCoroutine(HitStopCoroutine(duration));
        }

        public void PlayEffect()
        {
            Stop(defaultDuration);
        }

        public void StopEffect()
        {
            if (!isHitStopping) return;

            StopAllCoroutines();

            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            isHitStopping = false;
        }

        private IEnumerator HitStopCoroutine(float duration)
        {
            isHitStopping = true;

            float originalTimeScale = Time.timeScale;
            float originalFixedDelta = Time.fixedDeltaTime;

            Time.timeScale = 0f;
            Time.fixedDeltaTime = 0f;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = originalFixedDelta;

            isHitStopping = false;
        }
    }
}