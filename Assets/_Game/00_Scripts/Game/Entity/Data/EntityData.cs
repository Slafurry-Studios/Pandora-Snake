using Slafurry.Utils.Attributes;
using UnityEngine;

namespace Game.Entities
{
    [GameAssetCreator ("Entity/Non Hostile", "Civilian", order: 4)]
    public class EntityData : ScriptableObject
    {
        [Header("Basic Stats")]
        [SerializeField] private float health;
        [SerializeField] private float moveSpeed;


        [Header("Combat")]
        [SerializeField] private BulletData bulletFireData;
        [SerializeField] private StreamChatType deathChatType;

        [Header("Audio")]
        [SerializeField] private string audioCategory;
        [SerializeField] private string shootSFX;

        public float Health => health;
        public float MoveSpeed => moveSpeed;

        public BulletData BulletFireData => bulletFireData;
        public StreamChatType DeathChatType => deathChatType;

        public string AudioCategory => audioCategory;
        public string ShootSFX => shootSFX;
    }

}