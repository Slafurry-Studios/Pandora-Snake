using UnityEngine;

namespace Game.Player
{
    public class MarkerHistoryBuffer
    {
        public struct Marker
        {
            public Vector3 position;
            public Quaternion rotation;

            public Marker(Vector3 pos, Quaternion rot)
            {
                position = pos;
                rotation = rot;
            }
        }

        private Marker[] history;
        private int headIndex;
        private int count;

        public int Count => count;

        public MarkerHistoryBuffer(int initialSize)
        {
            history = new Marker[Mathf.Max(1, initialSize)];
            headIndex = 0;
            count = 0;
        }

        public void EnsureCapacity(int requiredSize)
        {
            if (history.Length >= requiredSize)
                return;

            Resize(requiredSize * 2);
        }

        public void Record(Vector3 position, Quaternion rotation)
        {
            headIndex--;
            if (headIndex < 0)
                headIndex = history.Length - 1;

            history[headIndex] = new Marker(position, rotation);

            if (count < history.Length)
                count++;
        }

        public Marker Get(int offset)
        {
            if (offset >= count)
                offset = count - 1;

            if (offset < 0)
                offset = 0;

            int index = (headIndex + offset) % history.Length;
            return history[index];
        }

        private void Resize(int newSize)
        {
            Marker[] oldHistory = history;
            int oldHeadIndex = headIndex;
            int oldCount = count;

            Marker[] newHistory = new Marker[newSize];
            int newCount = Mathf.Min(oldCount, newSize);

            for (int i = 0; i < newCount; i++)
            {
                int oldIndex = (oldHeadIndex + i) % oldHistory.Length;
                newHistory[i] = oldHistory[oldIndex];
            }

            history = newHistory;
            headIndex = 0;
            count = newCount;
        }
    }
}