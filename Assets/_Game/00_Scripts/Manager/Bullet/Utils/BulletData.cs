using Game.Gameplay;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public struct BulletData
{
    [Header("Basic")]
    public Bullet prefab;
    public float damage;
    public float speed;
    public float hitRadius;

    [Header("Behaviour")]
    public LayerMask targetMask;
    public float maxDistance;
    public float scale;


    [Header("Special Bullet")]
    public bool isExplosive;
    public bool isRichochet;

    [HideInInspector] public Vector2 startPos;
    [HideInInspector] public Vector2 direction;
}
}