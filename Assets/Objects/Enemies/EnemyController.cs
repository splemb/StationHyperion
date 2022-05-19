using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : ShootInteraction
{
    public float health;
    public float maxHealth = 10f;

    public void Start()
    {
        health = maxHealth;
    }

    public override void Shot(float damage)
    {
        base.Shot(damage);
        health -= damage;
    }

    public void Update()
    {
        if (health <= 0) gameObject.SetActive(false);
    }

    public void Reset()
    {
        health = maxHealth;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        Reset();
    }
}
