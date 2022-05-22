using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : ShootInteraction
{
    public float health;
    public float maxHealth = 10f;

    [SerializeField] MeshRenderer m_Renderer;

    [SerializeField] ParticleSystem hitParticle;
    [SerializeField] ParticleSystem deathParticle;

    Vector3 initialPos;

    public void Awake()
    {
        health = maxHealth;
        initialPos = transform.localPosition;
    }

    public override void Shot(float damage)
    {
        base.Shot(damage);
        health -= damage;
        hitParticle.Play();

        if (health <= 0)
        {
            deathParticle.Play();
            m_Renderer.enabled = false;
            StartCoroutine(DisableBehaviour());
        }
    }

    public void Reset()
    {
        health = maxHealth;
        gameObject.SetActive(true);
        m_Renderer.enabled = true;
        transform.localPosition = initialPos;
        
    }

    private void OnEnable()
    {
        Reset();
    }

    IEnumerator DisableBehaviour()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
