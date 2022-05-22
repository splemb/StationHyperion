using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootInteraction : MonoBehaviour
{
    public virtual void Shot(float damage = 0f)
    {
        //Debug.Log(gameObject.name + " was shot for " + damage.ToString() + " damage!");
    }
}
