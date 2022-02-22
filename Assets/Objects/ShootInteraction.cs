using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootInteraction : MonoBehaviour
{
    public virtual void Shot()
    {
        Debug.Log(gameObject.name + " was shot!");
    }
}
