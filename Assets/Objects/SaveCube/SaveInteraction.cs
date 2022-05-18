using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInteraction : ShootInteraction
{
    public Transform respawnPoint;
    public GameObject room;

    [SerializeField] AudioSource audioSource;

    public override void Shot(float damage)
    {
        base.Shot(damage);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().saveData.respawnPoint = this.name;

        GetComponent<Animator>().SetTrigger("Bounce");

        audioSource.Play();

        SerializationManager.Save("file1", GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().saveData);
    }
}
