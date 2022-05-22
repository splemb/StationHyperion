using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [SerializeField] float dmg = 10f;

    private void OnTriggerStay(Collider other)
    {
        if (other == null) return;

        switch (other.tag)
        {
            case "Player":
                other.GetComponent<PlayerBehaviour>().TakeDamage(dmg);
                break;
        }
    }
}
