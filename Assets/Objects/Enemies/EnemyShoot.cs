using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] GameObject shot;
    EnemyController enemy;

    Transform target;

    float nextShot;
    float shotDelay = 1f;

    private void OnEnable()
    {
        enemy = GetComponent<EnemyController>();
        target = Camera.main.transform;
        nextShot = Time.time + shotDelay;

    }

    private void FixedUpdate()
    {
        

        if (Time.time > nextShot && enemy.health > 0 && ((target.position - transform.position).magnitude < 30f))
        {
            GameObject newShot = Instantiate(shot, transform.position, Quaternion.identity);
            newShot.transform.LookAt(target);

            nextShot = Time.time + shotDelay;
        }

    }
}
