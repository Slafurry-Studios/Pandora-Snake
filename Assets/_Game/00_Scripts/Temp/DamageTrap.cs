using UnityEngine;
using Game.Player; // This is required so the script knows what your PlayerHealth is!

public class DamageTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private float damageAmount = 15f;
    [SerializeField] private bool destroyOnImpact = false;

    // This built-in Unity method fires the moment two 2D colliders touch
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object that bumped into this triangle has your PlayerHealth script
        PlayerHealth snekHealth = collision.gameObject.GetComponent<PlayerHealth>();

        // If snekHealth is NOT null, it means we definitely hit the player!
        if (snekHealth != null)
        {
            // Deal the damage
            snekHealth.TakeDamage(damageAmount);
            Debug.Log($"Bonk! The snek took {damageAmount} damage.");

            // Optional: Destroy the triangle after it deals damage
            if (destroyOnImpact)
            {
                Destroy(gameObject);
            }
        }
    }
}