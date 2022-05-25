using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : ShootInteraction
{
    public float health;
    public float maxHealth = 10f;

    [SerializeField] MeshRenderer m_Renderer;
    [SerializeField] GameObject trail;
    [SerializeField] GameObject hurtBox;

    [SerializeField] ParticleSystem hitParticle;
    [SerializeField] ParticleSystem deathParticle;

    [SerializeField] Material original;
    [SerializeField] Material hitMaterial;

    [SerializeField] float invincible = 0f;

    [SerializeField] AudioSource hitSound;

    Rigidbody rb;

    Vector3 initialPos;

    public void Awake()
    {
        health = maxHealth;
        initialPos = transform.localPosition;
        original = m_Renderer.material;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (invincible > 0)
        {
            invincible -= Time.deltaTime;
            m_Renderer.material = hitMaterial;
        }
        else m_Renderer.material = original;
    }

    public override void Shot(float damage)
    {
        if (invincible > 0) return;
        base.Shot(damage);
        health -= damage;
        hitParticle.Play();
        m_Renderer.material = hitMaterial;
        invincible = 0.1f;
        

        if (health <= 0)
        {
            deathParticle.Play();
            m_Renderer.enabled = false;
            hurtBox.SetActive(false);
            trail.SetActive(false);
            hitSound.pitch = 0.2f + Random.Range(-0.1f, 0.1f);
            hitSound.Play();
            StartCoroutine(DisableBehaviour());
        } else
        {
            hitSound.pitch = 0.7f + Random.Range(-0.3f, 0.3f);
            hitSound.Play();

            switch (Random.Range(0, 5))
            {
                case 0:
                    rb.AddForce(Vector3.right * 10f);
                    break;
                case 1:
                    rb.AddForce(Vector3.forward * 10f);
                    break;
                case 2:
                    rb.AddForce(-Vector3.forward * 10f);
                    break;
                case 3:
                    rb.AddForce(Vector3.up * 10f);
                    break;
                case 4:
                    rb.AddForce(-Vector3.up * 10f);
                    break;
            }
        }
    }

    public void Reset()
    {
        health = maxHealth;
        gameObject.SetActive(true);
        m_Renderer.enabled = true;
        hurtBox.SetActive(true);
        trail.SetActive(true);
        transform.localPosition = initialPos;
        m_Renderer.material = original;
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
