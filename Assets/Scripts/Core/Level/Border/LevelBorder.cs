using UnityEngine;
using UnityEngine.Events;

public class LevelBorder : MonoBehaviour
{
    [SerializeField] private UnityEvent OnBorderEntered;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) OnBorderEntered?.Invoke();
    }
}