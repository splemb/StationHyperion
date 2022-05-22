using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    PlayerBehaviour playerBehaviour;
    [SerializeField] CanvasGroup fadeCanvas;
    [SerializeField] Slider HPSlider;
    [SerializeField] Image crosshair;
    [SerializeField] Image crosshair_up;
    [SerializeField] TMPro.TextMeshProUGUI powerDisplay;
    [SerializeField] GameObject pauseCanvas;

    private void Start()
    {
        playerBehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        HPSlider.maxValue = playerBehaviour.maxHealth;
    }

    private void Update()
    {
        HPSlider.value = Mathf.Lerp(HPSlider.value, playerBehaviour.health, Time.deltaTime * 10f);

        pauseCanvas.SetActive(PauseManager.GAME_IS_PAUSED);

        RaycastHit hit;
        Transform cameraTransform = Camera.main.transform;
        LayerMask environmentMask = playerBehaviour.environmentMask;

        crosshair.enabled = false;
        crosshair_up.enabled = false;

        powerDisplay.text = playerBehaviour.atkPower.ToString("F2");

        if (playerBehaviour.saveData.GrapplingHook)
        {

            Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 20f, environmentMask);
            if (hit.collider != null)
            {
                if (hit.collider.tag != "CantGrapple") crosshair.enabled = true;
            }

            Physics.Raycast(cameraTransform.position, playerBehaviour.transform.forward * 0.4f + Vector3.up, out hit, 20f, environmentMask);
            if (hit.collider != null)
            {
                if (hit.collider.tag != "CantGrapple") { crosshair_up.enabled = true; }
            }
        }

    }

}
