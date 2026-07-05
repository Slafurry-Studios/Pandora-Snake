using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MenuParallax : MonoBehaviour
{
    [Header("Parallax")]
    [SerializeField] private float offsetMultiplier = 50f;
    [SerializeField] private float smoothTime = 0.2f;

    private RectTransform rectTransform;

    private Vector2 startPosition;
    private Vector2 velocity;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        startPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        // Mouse dalam viewport (0 - 1)
        Vector2 mouse =
            Camera.main.ScreenToViewportPoint(Input.mousePosition);

        // Ubah menjadi (-0.5 s/d 0.5)
        mouse -= Vector2.one * 0.5f;

        Vector2 target =
            startPosition + mouse * offsetMultiplier;

        rectTransform.anchoredPosition =
            Vector2.SmoothDamp(
                rectTransform.anchoredPosition,
                target,
                ref velocity,
                smoothTime
            );
    }
}