using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] GameObject shot;
    [SerializeField] AudioSource shotSound;
    EnemyController enemy;

    Transform target;

    float nextShot;
    [SerializeField] float shotDelay = 1f;
    [SerializeField] LayerMask shotMask;

    private void OnEnable()
    {
        enemy = GetComponent<EnemyController>();
        target = Camera.main.transform;
        nextShot = Time.time + shotDelay;

    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, (target.position - transform.position).normalized, out hit, 50f, shotMask))
        {

            if (hit.collider.tag == "Player")
            {
                if (Time.time > nextShot && enemy.health > 0)
                {
                    shotSound.Play();
                    GameObject newShot = Instantiate(shot, transform.position, Quaternion.identity);
                    newShot.transform.LookAt(target);

                    nextShot = Time.time + shotDelay;
                }
            }
        }
    }
}
