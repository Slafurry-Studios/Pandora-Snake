using UnityEngine;

namespace Game.Entities
{
    public interface IEntityShoot
    {
        void Initialize(EntityBrain entityBrain, BulletData bulletFireData, Animator animator);
        void InitKeys(string sfxCategory, string shootSFX);
        void Shoot(Vector3 direction);
    }
}