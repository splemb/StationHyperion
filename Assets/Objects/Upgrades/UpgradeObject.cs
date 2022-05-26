using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeObject : ShootInteraction
{
    public enum UpgradeType { Health, Damage }
    [SerializeField] public UpgradeType upgradeType;
    PlayerBehaviour player;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] AudioSource audioSource;
    [SerializeField] ParticleSystem particleSystem;
    bool stopChecking;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        stopChecking = false;
        meshRenderer.enabled = true;
        GetComponent<SphereCollider>().enabled = true;
    }

    private void FixedUpdate()
    {
        if (!stopChecking)
        {
            foreach (string str in player.saveData.collectedObjects)
            {
                if (str == gameObject.name) gameObject.SetActive(false);
            }
        }
    }

    public override void Shot(float damage)
    {
        base.Shot(damage);
        player.saveData.collectedObjects.Add(name);

        switch (upgradeType)
        {
            case UpgradeType.Health:
                GameObject.Find("MessageCanvas").GetComponent<Message>().DisplayMessage("MAX HEALTH INCREASED");
                player.saveData.maxHealth += 10f;
                player.health = player.saveData.maxHealth;
                break;
            case UpgradeType.Damage:
                GameObject.Find("MessageCanvas").GetComponent<Message>().DisplayMessage("BASE POWER INCREASED");
                player.saveData.baseDamage += 1f;
                player.atkPower = player.saveData.baseDamage;
                break;
        }

        particleSystem.Play();
        audioSource.Play();
        stopChecking = true;
        meshRenderer.enabled = false;
        GetComponent<SphereCollider>().enabled = false;

        //StartCoroutine(DisableBehaviour());
    }

    IEnumerator DisableBehaviour()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
