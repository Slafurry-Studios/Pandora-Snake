using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PlayerTail : MonoBehaviour
    {
        [Header("Tail Settings")]
        [SerializeField] private GameObject bodyPrefab;
        [SerializeField] private GameObject tailPrefab;
        [SerializeField] private GameObject saddlePrefab;
        [SerializeField] private int initialBodySize = 3;

        [Tooltip("Jarak minimum antar marker yang direkam (menentukan resolusi/kehalusan ekor)")]
        [SerializeField] private float minRecordDistance = 0.02f;

        [Tooltip("Jarak dunia (world-space) yang diinginkan antar tiap segmen body/tail")]
        [SerializeField] private float segmentSpacing = 0.5f;

        [Tooltip("Posisi saddle di antara head dan body pertama. 0 = nempel head, 1 = nempel body pertama")]
        [Range(0f, 1f)]
        [SerializeField] private float saddlePositionRatio = 0.5f;

        [Header("Rendering")]
        [Tooltip("Sorting order untuk head/segmen terdepan. Semakin dekat ke tail, semakin kecil (di-render di bawah).")]
        [SerializeField] private int baseSortingOrder = 100;

        // historyGap dihitung otomatis dari segmentSpacing / minRecordDistance,
        // jadi jarak antar segmen selalu konsisten walau minRecordDistance diubah.
        private int historyGap = 1;

        private List<Transform> bodyParts = new List<Transform>();

        private Transform tail;
        private Transform saddle;
        private Transform tailContainer;

        private Marker[] positionHistory;
        private int historyIndex = 0;
        private int historyCount = 0;
        private Player player;
        private bool initialized;

        private struct Marker
        {
            public Vector3 position;
            public Quaternion rotation;

            public Marker(Vector3 pos, Quaternion rot)
            {
                position = pos;
                rotation = rot;
            }
        }

        public void Initialize()
        {
            player = GetComponentInParent<Player>();
            RecalculateHistoryGap();

            tailContainer =
                new GameObject("TailContainer_" + gameObject.name).transform;

            AllocateHistory(
                Mathf.Max(100, (initialBodySize + 3) * historyGap)
            );

            RecordMarker();

            SpawnInitialBody();

            CreateSaddle();
            CreateTail();
            initialized = true;
        }

        private void Update()
        {
            RecordMovement();
            UpdateBodyParts();
            UpdateSaddle();
            UpdateTail();
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
            if (historyCount == 0)
            {
                RecordMarker();
                return;
            }

            Vector3 lastPos = GetMarker(0).position;
            Quaternion lastRot = GetMarker(0).rotation;

            Vector3 delta = player.transform.position - lastPos;
            float dist = delta.magnitude;

            if (dist < minRecordDistance)
                return;

            // Subdivide: rekam beberapa marker sepanjang lintasan dengan jarak
            // ~minRecordDistance, bukan cuma satu marker di posisi akhir frame.
            // Ini mencegah ekor "melar"/kepotong saat kecepatan naik (sprint),
            // karena jarak antar marker jadi konsisten terlepas dari deltaTime/speed.
            Vector3 dir = delta / dist;
            int steps = Mathf.FloorToInt(dist / minRecordDistance);

            // Batasi jumlah step per frame supaya tidak meledak kalau ada lag spike ekstrem
            // (misal object di-teleport jauh). Sesuaikan angka ini kalau perlu.
            const int maxStepsPerFrame = 64;
            steps = Mathf.Min(steps, maxStepsPerFrame);

            for (int i = 1; i <= steps; i++)
            {
                float stepDist = minRecordDistance * i;
                float t = stepDist / dist;

                Vector3 pos = lastPos + dir * stepDist;
                Quaternion rot = Quaternion.Slerp(lastRot, player.transform.rotation, t);

                RecordMarkerAt(pos, rot);
            }
        }

        private void CreateSaddle()
        {
            if (saddle != null || saddlePrefab == null)
                return;

            Vector3 spawnPos =
                GetSpawnBehindHeadFraction(saddlePositionRatio);

            GameObject obj =
                Instantiate(
                    saddlePrefab,
                    spawnPos,
                    player.transform.rotation,
                    tailContainer
                );

            saddle = obj.transform;

            // Saddle harus render tepat di bawah head, di atas body pertama
            ApplySortingOrder(saddle, baseSortingOrder - 1);
        }

        private void UpdateSaddle()
        {
            if (saddle == null)
                return;

            // Offset pecahan antara head (offset 0) dan body pertama (offset historyGap)
            int offset = Mathf.RoundToInt(historyGap * saddlePositionRatio);

            if (offset < historyCount)
            {
                Marker marker = GetMarker(offset);

                saddle.position = marker.position;
                saddle.rotation = marker.rotation;
            }
        }

        private void CreateTail()
        {
            if (tail != null)
                return;

            Vector3 spawnPos =
                GetSpawnBehindHead(bodyParts.Count);

            GameObject obj =
                Instantiate(
                    tailPrefab,
                    spawnPos,
                    player.transform.rotation,
                    tailContainer
                );

            tail = obj.transform;

            // Tail paling belakang = order paling kecil
            ApplySortingOrder(tail, baseSortingOrder - 2 - bodyParts.Count);
        }

        private void UpdateTail()
        {
            if (tail == null)
                return;

            int offset =
                (bodyParts.Count + 1) * historyGap;

            if (offset < historyCount)
            {
                Marker marker = GetMarker(offset);

                tail.position = marker.position;
                tail.rotation = marker.rotation;
            }
        }

        private void UpdateBodyParts()
        {
            for (int i = 0; i < bodyParts.Count; i++)
            {
                int offset =
                    (i + 1) * historyGap;

                if (offset < historyCount)
                {
                    Marker marker =
                        GetMarker(offset);

                    bodyParts[i].position =
                        marker.position;

                    bodyParts[i].rotation =
                        marker.rotation;
                }
            }
        }

        public void Grow(int amount = 1)
        {
            // Pastikan buffer history cukup besar untuk jumlah segmen baru
            // SEBELUM instantiate, supaya tidak ada frame dengan data marker kosong.
            int requiredAfterGrowth =
                (bodyParts.Count + amount + 3) * historyGap;

            if (positionHistory.Length < requiredAfterGrowth)
            {
                AllocateHistory(requiredAfterGrowth * 2);
            }

            for (int i = 0; i < amount; i++)
            {
                Marker marker =
                    GetMarker(
                        (bodyParts.Count + 1)
                        * historyGap
                    );

                GameObject body =
                    Instantiate(
                        bodyPrefab,
                        marker.position,
                        marker.rotation,
                        tailContainer
                    );

                bodyParts.Add(body.transform);

                // Sorting order sesuai posisi terakhir di rantai (index = bodyParts.Count - 1)
                ApplySortingOrder(body.transform, baseSortingOrder - 2 - (bodyParts.Count - 1));
            }

            // Tail sekarang di belakang lebih jauh lagi, order-nya perlu di-update juga
            if (tail != null)
            {
                ApplySortingOrder(tail, baseSortingOrder - 2 - bodyParts.Count);
            }
        }

        private void AllocateHistory(int size)
        {
            Marker[] oldHistory = positionHistory;
            int oldHistoryIndex = historyIndex;
            int oldHistoryCount = historyCount;

            Marker[] newHistory = new Marker[size];
            int newHistoryCount = Mathf.Min(oldHistoryCount, size);

            if (oldHistory != null)
            {
                for (int i = 0; i < newHistoryCount; i++)
                {
                    int oldIndex = (oldHistoryIndex + i) % oldHistory.Length;
                    newHistory[i] = oldHistory[oldIndex];
                }
            }

            positionHistory = newHistory;
            historyIndex = 0;
            historyCount = newHistoryCount;
        }

        private void RecordMarker()
        {
            if (!initialized) return;
            RecordMarkerAt(player.transform.position, player.transform.rotation);
        }

        private void RecordMarkerAt(Vector3 pos, Quaternion rot)
        {
            int required =
                (bodyParts.Count + 3)
                * historyGap;

            if (positionHistory.Length < required)
            {
                AllocateHistory(required * 2);
            }

            historyIndex--;

            if (historyIndex < 0)
                historyIndex =
                    positionHistory.Length - 1;

            positionHistory[historyIndex] =
                new Marker(pos, rot);

            if (historyCount < positionHistory.Length)
                historyCount++;
        }

        private Marker GetMarker(int offset)
        {
            if (offset >= historyCount)
                offset = historyCount - 1;

            int index =
                (historyIndex + offset)
                % positionHistory.Length;

            return positionHistory[index];
        }

        private Vector3 GetSpawnBehindHead(int index)
        {
            Vector3 backward =
                -player.transform.up * segmentSpacing * (index + 1);

            return player.transform.position + backward;
        }

        private Vector3 GetSpawnBehindHeadFraction(float fraction)
        {
            Vector3 backward =
                -player.transform.up * segmentSpacing * fraction;

            return player.transform.position + backward;
        }

        private void SpawnInitialBody()
        {
            for (int i = 0; i < initialBodySize; i++)
            {
                Vector3 spawnPos =
                    GetSpawnBehindHead(i);

                GameObject body =
                    Instantiate(
                        bodyPrefab,
                        spawnPos,
                        player.transform.rotation,
                        tailContainer
                    );
                bodyParts.Add(body.transform);

                // Body index 0 = paling dekat head, jadi order-nya harus lebih tinggi
                // dari body berikutnya. Mulai 2 di bawah saddle.
                ApplySortingOrder(body.transform, baseSortingOrder - 2 - i);
            }
        }

        private void ApplySortingOrder(Transform target, int order)
        {
            if (target == null)
                return;

            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = order;
                return;
            }

            // Fallback: cek child kalau sprite ada di object anak, bukan di root prefab
            SpriteRenderer childSr = target.GetComponentInChildren<SpriteRenderer>();
            if (childSr != null)
            {
                childSr.sortingOrder = order;
            }
        }
    

    
    }

    
}