using UnityEngine;

namespace Game.Player
{
    public class SnakeTailManager : MonoBehaviour
    {
        [Header("Tail Settings")]
        [SerializeField] private GameObject bodyPrefab;
        [SerializeField] private GameObject tailPrefab;
        [SerializeField] private GameObject saddlePrefab;
        [SerializeField] private int initialBodySize = 3;

        [SerializeField] private float minRecordDistance = 0.02f;

        [SerializeField] private float segmentSpacing = 0.5f;

        [Range(0f, 1f)]
        [SerializeField] private float saddlePositionRatio = 0.5f;

        [Header("Rendering")]
        [SerializeField] private int baseSortingOrder = 100;

        private const int SaddleSortingOffset = 1;
        private const int BodySortingOffset = 2;
        private const int MaxStepsPerFrame = 64;

        private int historyGap = 1;

        private Transform tailContainer;

        private MarkerHistoryBuffer history;
        private SnakeBodyChain bodyChain;
        private SnakeSaddleController saddleController;
        private SnakeTailTipController tailTipController;

        private void Start()
        {
            RecalculateHistoryGap();

            tailContainer = new GameObject("TailContainer_" + gameObject.name).transform;

            history = new MarkerHistoryBuffer(Mathf.Max(100, (initialBodySize + 3) * historyGap));
            history.Record(transform.position, transform.rotation);

            bodyChain = new SnakeBodyChain(bodyPrefab, tailContainer, baseSortingOrder, BodySortingOffset);
            bodyChain.SpawnInitial(initialBodySize, GetSpawnBehindHead, transform.rotation);

            saddleController = new SnakeSaddleController(saddlePrefab, tailContainer, baseSortingOrder - SaddleSortingOffset);
            saddleController.Spawn(GetSpawnBehindHeadFraction(saddlePositionRatio), transform.rotation);

            tailTipController = new SnakeTailTipController(tailPrefab, tailContainer, baseSortingOrder, BodySortingOffset);
            tailTipController.Spawn(GetSpawnBehindHead(bodyChain.Count), transform.rotation, bodyChain.Count);
        }

        private void Update()
        {
            RecordMovement();

            bodyChain.UpdateFromHistory(history, historyGap);

            int saddleOffset = Mathf.RoundToInt(historyGap * saddlePositionRatio);
            saddleController.UpdateFromHistory(history, saddleOffset);

            int tailOffset = (bodyChain.Count + 1) * historyGap;
            tailTipController.UpdateFromHistory(history, tailOffset);
        }

        public void Grow(int amount = 1)
        {
            int requiredAfterGrowth = (bodyChain.Count + amount + 3) * historyGap;
            history.EnsureCapacity(requiredAfterGrowth);

            bodyChain.Grow(amount, history, historyGap);

            tailTipController.RefreshSortingOrder(bodyChain.Count);
        }

        private void RecalculateHistoryGap()
        {
            historyGap = Mathf.Max(
                1,
                Mathf.RoundToInt(segmentSpacing / Mathf.Max(minRecordDistance, 0.0001f))
            );
        }

        private void RecordMovement()
        {
            if (history.Count == 0)
            {
                history.Record(transform.position, transform.rotation);
                return;
            }

            MarkerHistoryBuffer.Marker last = history.Get(0);

            Vector3 delta = transform.position - last.position;
            float dist = delta.magnitude;

            if (dist < minRecordDistance)
                return;

            Vector3 dir = delta / dist;
            int steps = Mathf.Min(Mathf.FloorToInt(dist / minRecordDistance), MaxStepsPerFrame);

            for (int i = 1; i <= steps; i++)
            {
                float stepDist = minRecordDistance * i;
                float t = stepDist / dist;

                Vector3 pos = last.position + dir * stepDist;
                Quaternion rot = Quaternion.Slerp(last.rotation, transform.rotation, t);

                EnsureHistoryCapacityForCurrentChain();
                history.Record(pos, rot);
            }
        }

        private void EnsureHistoryCapacityForCurrentChain()
        {
            int required = (bodyChain.Count + 3) * historyGap;
            history.EnsureCapacity(required);
        }

        private Vector3 GetSpawnBehindHead(int index)
        {
            Vector3 backward = -transform.up * segmentSpacing * (index + 1);
            return transform.position + backward;
        }

        private Vector3 GetSpawnBehindHeadFraction(float fraction)
        {
            Vector3 backward = -transform.up * segmentSpacing * fraction;
            return transform.position + backward;
        }
    }
}