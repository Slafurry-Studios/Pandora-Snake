using Game.AI.Boss;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthHUD : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject bossHealthPanel;
    private float max;

    public void FollowEvent(float max)
    {
        this.max = max;
        FindAnyObjectByType<BossHealth>().OnHealthChanged += UpdateHealth;
    }

    public void UpdateHealth(float current)
    {
        bossHealthPanel.SetActive(true);
        healthSlider.value = current / max;
    }
}