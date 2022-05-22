using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCubeInteraction : ShootInteraction
{
    [SerializeField] AudioSource audioSource;

    public enum Items { Grapple, Slide, Dash, Stomp, WallJump }
    [SerializeField] public Items item;
    [SerializeField] ParticleSystem particles;
    [SerializeField] MeshRenderer meshRenderer;
    string itemName = "";

    private void OnEnable()
    {
        GetComponent<SphereCollider>().enabled = true;
        meshRenderer.enabled = true;
    }

    public override void Shot(float damage)
    {
        base.Shot(damage);

        PlayerBehaviour player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();

        audioSource.Play();

        MarkCollected();

        GameObject.Find("MessageCanvas").GetComponent<Message>().DisplayMessage(itemName + " Acquired");

        GetComponent<SphereCollider>().enabled = false;
        meshRenderer.enabled = false;

        particles.Play();

        StartCoroutine(DisableBehaviour());
    }

    void MarkCollected()
    {
        PlayerBehaviour player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();

        switch (item)
        {
            case Items.Grapple:
                player.saveData.GrapplingHook = true;
                itemName = "Grapple Beam";
                break;
            case Items.Slide:
                player.saveData.Slide = true;
                itemName = "Power Slide";
                break;
            case Items.Dash:
                player.saveData.Dash = true;
                itemName = "Air Dash";
                break;
            case Items.Stomp:
                player.saveData.Stomp = true;
                itemName = "Quake Stomp";
                break;
            case Items.WallJump:
                player.saveData.WallJump = true;
                itemName = "Wall Jump";
                break;
        }
    }

    IEnumerator DisableBehaviour()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
