using Game.Entities;
using Game.Gameplay;
using Slafurry.System.Audio;
using UnityEngine;

namespace Game.Entity
{
    public class EntityShoot : MonoBehaviour, IEntityShoot
    {
        private EntityBrain entityBrain;
        private BulletData bulletFireData;
        private Animator animator;
        private string sfxCategory;
        private string shootSFX;
        private string attackAnim;

        public void Initialize(EntityBrain entityBrain, BulletData bulletFireData, Animator animator)
        {
            this.bulletFireData = bulletFireData;
            this.entityBrain = entityBrain;
            this.animator = animator;
        }

        public void InitKeys(string sfxCategory, string shootSFX)
        {
            this.sfxCategory = sfxCategory;
            this.shootSFX = shootSFX;
        }
        public void Shoot(Vector3 direction)
        {
            bulletFireData.direction = direction;

            GameManager.Bullet.FireBullet(bulletFireData);

            Audio.PlaySFX2D(sfxCategory, shootSFX);


            if (animator != null && !string.IsNullOrEmpty(attackAnim))
            {
                animator.ResetTrigger(attackAnim);
                animator.SetTrigger(attackAnim);
            }
        }
    }
}