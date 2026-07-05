using UnityEngine;
using Game.Data;

namespace Game.Gameplay
{
    public class LootDropper : MonoBehaviour
    {
        [Header("Drop Settings")]
        [Tooltip("The Loot Table SO containing the items to drop.")]
        public LootDropData dropData;
        
        [Tooltip("The radius around the object to scatter the drops.")]
        public float scatterRadius = 1f;

        [Tooltip("If true, automatically drops loot when this object is destroyed (useful for enemies).")]
        public bool dropOnDestroy = false;

        public void DropLoot()
        {
            if (dropData == null || dropData.drops == null) return;

            foreach (var drop in dropData.drops)
            {
                if (drop.prefabToDrop == null) continue;

                if (Random.value <= drop.dropChance)
                {
                    for (int i = 0; i < drop.amount; i++)
                    {
                        Vector2 randomOffset = Random.insideUnitCircle * scatterRadius;
                        Vector3 targetPos = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

                        GameObject droppedItem = Instantiate(drop.prefabToDrop, transform.position, Quaternion.identity);

                        if (droppedItem.TryGetComponent<LootLauncher>(out var launcher))
                        {
                            launcher.Launch(transform.position, targetPos);
                        }
                        else
                        {
                            droppedItem.transform.position = targetPos;
                        }
                    }
                }
            }
        }
    }
}