using UnityEngine;
using Game.Gameplay;

namespace Game.Temp
{
    [RequireComponent(typeof(LootDropper))]
    [RequireComponent(typeof(Collider2D))]
    public class BuildingTest : MonoBehaviour
    {
        private LootDropper lootDropper;

        private void Awake()
        {
            lootDropper = GetComponent<LootDropper>();
        }

        private void OnMouseDown()
        {
            Debug.Log("Building clicked and destroyed!");
            
            if (lootDropper != null)
            {
                lootDropper.DropLoot();
            }

            Destroy(gameObject);
        }
    }
}
