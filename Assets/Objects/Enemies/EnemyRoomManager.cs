using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoomManager : MonoBehaviour
{
    private void OnEnable()
    {

        //Debug.Log("ENABLED");
        Component[] enemies = GetComponentsInChildren(typeof(EnemyController), true);

        foreach (Component e in enemies) {
            //Debug.Log(e.name);
            e.gameObject.SetActive(true);
        }
    }
}
