using UnityEngine;
using Game.Generic;
using Slafurry.System.Audio;

[System.Serializable]
public struct Building
{
    public Sprite buildingSprite;
    public int hitThreshold;
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BuildingHealth))]
public class BuildingSwitcher : MonoBehaviour
{
    [Header("Building")]
    public Building[] buildingPrefabs;

    [Header("Particle")]
    public GameObject[] destroyParticlePrefab;

    private BuildingHealth buildingHealth;
    private SpriteRenderer spriteRenderer;

    private int currentIndex = 0;
    private float hitCount = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildingHealth = GetComponent<BuildingHealth>();
    }

    void OnEnable()
    {
        buildingHealth.OnDamaged += HandleDamaged;
    }

    void OnDisable()
    {
        buildingHealth.OnDamaged -= HandleDamaged;
    }

    void Start()
    {
        if (buildingPrefabs.Length > 0)
        {
            spriteRenderer.sprite =
                buildingPrefabs[currentIndex].buildingSprite;
        }
    }

    void HandleDamaged(float amount)
    {
        hitCount += amount;
        CheckThreshold();
    }

    void CheckThreshold()
    {
        if (currentIndex >= buildingPrefabs.Length)
            return;

        if (hitCount >= buildingPrefabs[currentIndex].hitThreshold)
        {
            ChangeBuilding();
        }
    }

    void ChangeBuilding()
    {
        PlayDestroyEffect();

        currentIndex++;

        if (currentIndex < buildingPrefabs.Length)
        {
            spriteRenderer.sprite =
                buildingPrefabs[currentIndex].buildingSprite;
            Audio.PlaySFX2D("Building", "Partially_Destroyed");
        }
    }

    void PlayDestroyEffect()
    {
        if (destroyParticlePrefab == null || destroyParticlePrefab.Length == 0)
            return;

        int randomIndex = Random.Range(0, destroyParticlePrefab.Length);
        GameObject prefab = destroyParticlePrefab[randomIndex];

        if (!prefab)
            return;

        GameObject effect = Instantiate(
            prefab,
            transform.position,
            Quaternion.identity
        );

        ParticleSystem ps =
            effect.GetComponent<ParticleSystem>();

        if (ps)
        {
            ps.Play();

            Destroy(
                effect,
                ps.main.duration +
                ps.main.startLifetime.constantMax
            );
        }
        else
        {
            Destroy(effect, 3f);
        }
    }
}