using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatToTarget : MonoBehaviour
{
    Transform target;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        target = Camera.main.transform;
        
        rb.velocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if ((target.position - transform.position).magnitude < 30f)
            rb.AddForce((target.position - transform.position) * Time.deltaTime * 10f);
        else
        {
            switch (Random.Range(0, 5)) {
                case 0:
                    rb.AddForce(Vector3.right * Time.deltaTime * 10f);
                    break;
                case 1:
                    rb.AddForce(Vector3.forward * Time.deltaTime * 10f);
                    break;
                case 2:
                    rb.AddForce(-Vector3.forward * Time.deltaTime * 10f);
                    break;
                case 3:
                    rb.AddForce(Vector3.up * Time.deltaTime * 10f);
                    break;
                case 4:
                    rb.AddForce(-Vector3.up * Time.deltaTime * 10f);
                    break;
            }
        }
    }
}
