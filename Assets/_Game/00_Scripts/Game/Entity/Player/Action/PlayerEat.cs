using UnityEngine;
using Game.Gameplay;

namespace Game.Player
{
    public class PlayerEat : MonoBehaviour
    {
        [Header("Eating Settings")]
        [SerializeField] private float eatRange = 1.5f;
        [SerializeField] private LayerMask consumableLayer;

        private void Update()
        {
            EatConsumable();
        }

        private void EatConsumable()
        {
            Collider2D[] hitColliders =
                Physics2D.OverlapCircleAll(transform.position, eatRange, consumableLayer);

            foreach (var hitCollider in hitColliders)
            {
                CollectibleItem collectible = hitCollider.GetComponent<CollectibleItem>();
                if (collectible != null)
                {
                    Vector2 direction =
                        (transform.position - hitCollider.transform.position).normalized;

                    collectible.Consume(gameObject, direction);

                    Debug.Log($"[PlayerEat] Consumed: {hitCollider.name}");
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, eatRange);
        }
    }
}