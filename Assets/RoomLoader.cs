using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    [SerializeField] GameObject frontRoom;
    [SerializeField] GameObject backRoom;

    public void LoadRooms()
    {
        StopAllCoroutines();
        if (frontRoom) frontRoom.SetActive(true);
        if (backRoom) backRoom.SetActive(true);
    }

    public void UnloadRoom()
    {
        StartCoroutine(DelayUnloadRoom());
    }

    IEnumerator DelayUnloadRoom()
    {
        
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        yield return new WaitForSeconds(1f);

        if (transform.InverseTransformPoint(playerPos).z > 0) { if (backRoom) backRoom.SetActive(false); }
        else if (transform.InverseTransformPoint(playerPos).z < 0) { if (frontRoom) frontRoom.SetActive(false); }
    }

}
