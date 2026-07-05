using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class CollectibleItem : MonoBehaviour
    {
        [Header("Collectible Settings")]
        public List<AttributeModifier> attributes = new List<AttributeModifier>();

        [Header("Consume Animation Settings")]
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float pullDistance = 2f;

        [Header("Objective")]
        [SerializeField] private BaseObjectiveChannel[] consumeChannel;

        private SpriteRenderer spriteRenderer;
        private bool isConsumed = false;
        private Collider2D eatableCollider;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            eatableCollider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (isConsumed) return;

            isConsumed = true;

            Vector2 direction =
                (other.transform.position - transform.position).normalized;

            StartCoroutine(ConsumeRoutine(other.gameObject, direction));
        }

        protected virtual void OnConsumed()
        {
            Debug.Log($"[Eatable] {gameObject.name} has been consumed. Override OnConsumed() for custom behavior.");
        }

        public void Consume(GameObject player, Vector2 direction)
        {
            eatableCollider.enabled = false; // Disable the collider to prevent further interactions
            if (isConsumed) return;
            isConsumed = true;

            OnConsumed();

            Debug.Log($"[Eatable] {gameObject.name} has been consumed.");

            foreach (BaseObjectiveChannel channel in consumeChannel)
            {
                channel.Raise(1);
            }

            StartCoroutine(ConsumeRoutine(player, direction.normalized));
        }

        private IEnumerator ConsumeRoutine(GameObject player, Vector2 dir)
        {
            // 1. Apply attributes immediately
            foreach (var modifier in attributes)
            {
                if (modifier.attribute != null)
                {
                    modifier.attribute.Apply(player, modifier.amount);
                }
            }

            // 2. Animation setup
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + (Vector3)(dir * pullDistance);

            Vector3 startScale = transform.localScale;
            Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

            float elapsedTime = 0f;

            // 3. Consume animation (pull + shrink + fade)
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

                if (spriteRenderer != null)
                {
                    Color newColor = startColor;
                    newColor.a = Mathf.Lerp(startColor.a, 0f, t);
                    spriteRenderer.color = newColor;
                }

                yield return null;
            }

            transform.localScale = Vector3.zero;
            Destroy(gameObject);
        }
    }
}