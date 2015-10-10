using UnityEngine;
using System.Collections;
using System;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;
    protected bool dead;

    public event Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }
    public void TakeHit(float _damage, RaycastHit hit)
    {
        // do some stuff with hit var
        TakeDamage(_damage);
    }

    protected void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
        Debug.Log(gameObject.name);
    }

    public void TakeDamage(float _damage)
    {
        health -= _damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }
}
