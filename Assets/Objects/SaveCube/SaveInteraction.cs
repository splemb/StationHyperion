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

        PlayerBehaviour player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();

        player.saveData.respawnPoint = this.name;

        player.health = player.maxHealth;

        GetComponentInChildren<Animator>().SetTrigger("Bounce");

        audioSource.Play();

        SerializationManager.Save(FileNameTracker.FileName, GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().saveData);

        GameObject.Find("MessageCanvas").GetComponent<Message>().DisplayMessage("GAME SAVED");
    }
}
