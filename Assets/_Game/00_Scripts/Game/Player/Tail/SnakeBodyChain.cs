using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class SnakeBodyChain
    {
        private readonly GameObject bodyPrefab;
        private readonly Transform container;
        private readonly int baseSortingOrder;
        private readonly int sortingOffset;

        private readonly List<Transform> bodyParts = new List<Transform>();

        public int Count => bodyParts.Count;
        public IReadOnlyList<Transform> Parts => bodyParts;

        public SnakeBodyChain(GameObject bodyPrefab, Transform container, int baseSortingOrder, int sortingOffset)
        {
            this.bodyPrefab = bodyPrefab;
            this.container = container;
            this.baseSortingOrder = baseSortingOrder;
            this.sortingOffset = sortingOffset;
        }

        public void SpawnInitial(int amount, Func<int, Vector3> getSpawnPosition, Quaternion rotation)
        {
            for (int i = 0; i < amount; i++)
            {
                SpawnOne(getSpawnPosition(i), rotation);
            }
        }

        public void Grow(int amount, MarkerHistoryBuffer history, int historyGap)
        {
            for (int i = 0; i < amount; i++)
            {
                MarkerHistoryBuffer.Marker marker = history.Get((bodyParts.Count + 1) * historyGap);
                SpawnOne(marker.position, marker.rotation);
            }
        }

        public void UpdateFromHistory(MarkerHistoryBuffer history, int historyGap)
        {
            for (int i = 0; i < bodyParts.Count; i++)
            {
                int offset = (i + 1) * historyGap;

                if (offset < history.Count)
                {
                    MarkerHistoryBuffer.Marker marker = history.Get(offset);
                    bodyParts[i].position = marker.position;
                    bodyParts[i].rotation = marker.rotation;
                }
            }
        }

        private void SpawnOne(Vector3 position, Quaternion rotation)
        {
            GameObject body = UnityEngine.Object.Instantiate(bodyPrefab, position, rotation, container);
            bodyParts.Add(body.transform);

            SpriteSortingUtility.ApplySortingOrder(
                body.transform,
                baseSortingOrder - sortingOffset - (bodyParts.Count - 1)
            );
        }
    }
}