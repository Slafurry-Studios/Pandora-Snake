using UnityEngine;
using Game.Core; // Brings in IDamageable and IVisualEffect
using Game.Core.Effects;

namespace Game.Temp
{
    /// <summary>
    /// A temporary tester script to test hit effects.
    /// Attach this to an object with a Collider2D (e.g. BoxCollider2D) that is on the TargetLayer of your bullets.
    /// When hit by a bullet from the BulletManager, it will automatically trigger all attached IVisualEffects.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class EffectTester : MonoBehaviour, IDamageable
    {
        public void TakeDamage(float amount)
        {
            Debug.Log($"[EffectTester] Object '{gameObject.name}' took {amount} damage from a bullet! Triggering visual effects...");
            
            // Grab every script on this object (or its children) that implements the IVisualEffect interface
            IVisualEffect[] effects = GetComponentsInChildren<IVisualEffect>();
            
            // Loop through and play all of them!
            foreach (IVisualEffect effect in effects)
            {
                effect.PlayEffect();
            }
        }
    }
}
