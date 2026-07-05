using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CustomAlphaRaycast : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] 
    private float alphaThreshold = 0.1f; // Batas transparansi yang bisa diklik

    void Start()
    {
        // Mengambil komponen Image dari GameObject Button ini
        Image tombolImage = GetComponent<Image>();

        if (tombolImage != null)
        {
            // Mengatur area klik agar hanya mendeteksi pixel dengan alpha > threshold
            tombolImage.alphaHitTestMinimumThreshold = alphaThreshold;
        }
    }
}