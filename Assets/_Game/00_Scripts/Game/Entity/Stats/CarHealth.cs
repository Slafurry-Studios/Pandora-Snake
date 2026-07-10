using Game.Entities;
using Game.Managerd;
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

    public void CheckCollision(GameObject obj)
    {
        if (isDead)
            return;

        if (obj.CompareTag("Body"))
        {
            GetComponentInChildren<Rigidbody2D>().AddForce((transform.position - obj.transform.position).normalized * 1f, ForceMode2D.Impulse);
        }

        if (obj.GetComponentInChildren<EntityHealth>() != null && obj.CompareTag("Entity"))
        {
            obj.GetComponentInChildren<EntityHealth>().TakeDamage(99999999f);
            TakeDamage(collisionDmg);
        }

        if (obj.CompareTag("Building"))
        {
            obj.GetComponentInChildren<BuildingHealth>().TakeDamage(999999999f);
            TakeDamage(collisionDmg);
        }
    }

    protected override void Die()
    {
        base.Die();
        GameManagerOld.Instance.AddThreat(5);
    }
}