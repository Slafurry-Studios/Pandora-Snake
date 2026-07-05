using Game.Manager;
using UnityEngine;

public class CarHealth : EntityHealth
{
    [SerializeField] private float collisionDmg = 1f;

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        foreach (var effect in visualEffects)
        {
            effect.PlayEffect();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckCollision(other.gameObject);
    }

    private void CheckCollision(GameObject obj)
    {
        if (isDead)
            return;

        if (obj.CompareTag("Body"))
        {
            // knockback logic 
            GetComponent<Rigidbody2D>().AddForce((transform.position - obj.transform.position).normalized * 1f, ForceMode2D.Impulse);
        }

        if (obj.GetComponent<EntityHealth>() != null && obj.CompareTag("Entity"))
        {
            obj.GetComponent<EntityHealth>().TakeDamage(99999999f);
            TakeDamage(collisionDmg);
        }

        if (obj.CompareTag("Building"))
        {
            obj.GetComponent<BuildingHealth>().TakeDamage(999999999f);
            TakeDamage(collisionDmg);
        }
    }

    protected override void Die()
    {
        base.Die();
        GameManager.Instance.AddThreat(5);
    }
}