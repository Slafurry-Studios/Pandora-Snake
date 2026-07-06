using UnityEngine;

namespace Game.Player
{
    public class SnakeSaddleController
    {
        private readonly GameObject saddlePrefab;
        private readonly Transform container;
        private readonly int sortingOrder;

        private Transform saddle;

        public SnakeSaddleController(GameObject saddlePrefab, Transform container, int sortingOrder)
        {
            this.saddlePrefab = saddlePrefab;
            this.container = container;
            this.sortingOrder = sortingOrder;
        }

        public void Spawn(Vector3 spawnPosition, Quaternion rotation)
        {
            if (saddle != null || saddlePrefab == null)
                return;

            GameObject obj = Object.Instantiate(saddlePrefab, spawnPosition, rotation, container);
            saddle = obj.transform;

            SpriteSortingUtility.ApplySortingOrder(saddle, sortingOrder);
        }

        public void UpdateFromHistory(MarkerHistoryBuffer history, int offset)
        {
            if (saddle == null)
                return;

            if (offset < history.Count)
            {
                MarkerHistoryBuffer.Marker marker = history.Get(offset);
                saddle.position = marker.position;
                saddle.rotation = marker.rotation;
            }
        }
    }
}