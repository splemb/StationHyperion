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

    bool open = false;

    private void Start()
    {
        collision.enabled = true;
        roomLoader = GetComponent<RoomLoader>();
    }

    public override void Shot()
    {
        base.Shot();

        if (!open) Open();
    }

    void Open()
    {
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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && open)
        {
            StopAllCoroutines();
            Close();
        }
    }
}
