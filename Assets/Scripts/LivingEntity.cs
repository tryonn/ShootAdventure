using UnityEngine;
using System.Collections;
using System;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;
    protected bool dead;

    protected virtual void Start()
    {
        health = startingHealth;
    }
    public void TakeHit(float damage, RaycastHit hit)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Debug.Log("health: " + health);
            Die();
        }
    }

    protected void Die()
    {
        Debug.Log("Die");
        dead = true;
        GameObject.Destroy(gameObject);

        Debug.Log("Die.. Die");
    }
}
