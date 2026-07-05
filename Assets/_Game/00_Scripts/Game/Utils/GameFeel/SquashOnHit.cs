using UnityEngine;
using System.Collections;

namespace Game.Core.Effects
{
    public class SquashOnHit : MonoBehaviour, IVisualEffect
    {
        [SerializeField] private float squashX = 1.3f;
        [SerializeField] private float squashY = 0.7f;
        [SerializeField] private float duration = 0.15f;

        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        public void PlayEffect()
        {
            StopAllCoroutines();
            StartCoroutine(SquashRoutine());
        }

        public void StopEffect()
        {
            StopAllCoroutines();
            transform.localScale = _originalScale;
        }

        private IEnumerator SquashRoutine()
        {
            float half = duration / 2f;

            float elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / half;
                transform.localScale = Vector3.Lerp(_originalScale, new Vector3(
                    _originalScale.x * squashX,
                    _originalScale.y * squashY,
                    _originalScale.z), t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / half;
                transform.localScale = Vector3.Lerp(new Vector3(
                    _originalScale.x * squashX,
                    _originalScale.y * squashY,
                    _originalScale.z), _originalScale, t);
                yield return null;
            }

            transform.localScale = _originalScale;
        }
    }
}