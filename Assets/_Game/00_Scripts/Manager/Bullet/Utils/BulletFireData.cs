using Game.Gameplay;
using UnityEngine;

namespace Game
{
    public struct BulletFireData
{
    public Bullet prefab;
    public Vector2 startPos;
    public Vector2 direction;
    public float damage;
    public float speed;
    public float maxDistance;
    public LayerMask targetMask;
    public float hitRadius;
    public bool isExplosive;
    public bool isRichochet;
    public float scale;
}
}