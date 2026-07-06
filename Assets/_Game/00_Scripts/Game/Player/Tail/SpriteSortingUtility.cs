using UnityEngine;

namespace Game.Player
{
    public static class SpriteSortingUtility
    {
        public static void ApplySortingOrder(Transform target, int order)
        {
            if (target == null)
                return;

            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = order;
                return;
            }

            SpriteRenderer childSr = target.GetComponentInChildren<SpriteRenderer>();
            if (childSr != null)
            {
                childSr.sortingOrder = order;
            }
        }
    }
}