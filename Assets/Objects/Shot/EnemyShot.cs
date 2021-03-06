using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] LayerMask hitMask;
    [SerializeField] GameObject collision;

    public float speed = 40;
    public float damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position - transform.forward, transform.forward, out hit, 3f, hitMask)) {
            Collide(hit);
        }
    }

    void Collide(RaycastHit hit)
    {
        if (hit.collider.gameObject.GetComponent<PlayerBehaviour>())
        {
            hit.collider.gameObject.GetComponent<PlayerBehaviour>().TakeDamage(damage);
        }
        transform.position = hit.point;

        Instantiate(collision, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }
}
