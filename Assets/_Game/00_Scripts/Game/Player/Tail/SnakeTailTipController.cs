using UnityEngine;

namespace Game.Player
{
    public class SnakeTailTipController
    {
        private readonly GameObject tailPrefab;
        private readonly Transform container;
        private readonly int baseSortingOrder;
        private readonly int sortingOffset;

        private Transform tail;

        public SnakeTailTipController(GameObject tailPrefab, Transform container, int baseSortingOrder, int sortingOffset)
        {
            this.tailPrefab = tailPrefab;
            this.container = container;
            this.baseSortingOrder = baseSortingOrder;
            this.sortingOffset = sortingOffset;
        }

        public void Spawn(Vector3 spawnPosition, Quaternion rotation, int bodyCount)
        {
            if (tail != null)
                return;

            GameObject obj = Object.Instantiate(tailPrefab, spawnPosition, rotation, container);
            tail = obj.transform;

            SpriteSortingUtility.ApplySortingOrder(tail, baseSortingOrder - sortingOffset - bodyCount);
        }

        public void RefreshSortingOrder(int bodyCount)
        {
            if (tail == null)
                return;

            SpriteSortingUtility.ApplySortingOrder(tail, baseSortingOrder - sortingOffset - bodyCount);
        }

        public void UpdateFromHistory(MarkerHistoryBuffer history, int offset)
        {
            if (tail == null)
                return;

            if (offset < history.Count)
            {
                MarkerHistoryBuffer.Marker marker = history.Get(offset);
                tail.position = marker.position;
                tail.rotation = marker.rotation;
            }
        }
    }
}