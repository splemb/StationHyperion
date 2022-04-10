using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : ShootInteraction
{
    [SerializeField] Animator animator;
    [SerializeField] BoxCollider collision;
    [SerializeField] AudioSource audioSource;
    RoomLoader roomLoader;
    [SerializeField] AudioClip[] audioClips;
    Transform playerTransform;
    bool playerProximityFlag = false;

    bool open = false;

    private void Start()
    {
        collision.enabled = true;
        roomLoader = GetComponent<RoomLoader>();
    }

    private void FixedUpdate()
    {
        if (open)
        {
            if (!playerProximityFlag)
            {
                if ((transform.position - playerTransform.position).magnitude <= 2) playerProximityFlag = true;
            }
            else
            {
                if ((transform.position - playerTransform.position).magnitude > 2 && playerProximityFlag)
                {
                    StopAllCoroutines();
                    Close();
                }
            }
        }
    }

    public override void Shot(float damage)
    {
        base.Shot(damage);

        if (!open) Open();
    }

    void Open()
    {
        playerProximityFlag = false;
        if (playerTransform == null) playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (roomLoader) roomLoader.LoadRooms();
        audioSource.PlayOneShot(audioClips[0]);
        open = true;
        animator.SetBool("isOpen", open);
        collision.enabled = false;
        StartCoroutine(RecloseDelay());
    }

    void Close()
    {
        if (roomLoader) roomLoader.UnloadRoom();
        audioSource.PlayOneShot(audioClips[1]);
        open = false;
        animator.SetBool("isOpen", open);
        collision.enabled = true;
    }

    IEnumerator RecloseDelay()
    {
        yield return new WaitForSeconds(10f);
        Close();
    }
}
