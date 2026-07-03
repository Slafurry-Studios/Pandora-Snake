using Game.Player;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private Transform playerTransform;

    void Awake()
    {
        playerTransform = FindAnyObjectByType<PlayerHealth>().transform;
        TeleportPlayer();
    }

    void Start()
    {
    }
    public void TeleportPlayer()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player belum ditemukan, tidak bisa teleport.");
            return;
        }

        playerTransform.position = transform.position;
        playerTransform.rotation = transform.rotation;
    }
}