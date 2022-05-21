using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "Player":
                other.GetComponent<PlayerBehaviour>().TakeDamage(10f);
                break;
        }
    }
}
