using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseScreenManager : MonoBehaviour
{
    [SerializeField] GameObject[] upgradeBoxes;
    [SerializeField] AudioSource audioSource;
    PlayerBehaviour player;
    bool inputLock;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        audioSource.ignoreListenerPause = true;
        inputLock = false;
    }

    private void OnEnable()
    {
        if (player != null)
        {
            audioSource.Play();

            if (player.saveData.GrapplingHook) upgradeBoxes[0].SetActive(true);
            else upgradeBoxes[0].SetActive(false);

            if (player.saveData.Dash) upgradeBoxes[1].SetActive(true);
            else upgradeBoxes[1].SetActive(false);

            if (player.saveData.WallJump) upgradeBoxes[2].SetActive(true);
            else upgradeBoxes[2].SetActive(false);

            if (player.saveData.Slide) upgradeBoxes[3].SetActive(true);
            else upgradeBoxes[3].SetActive(false);

            if (player.saveData.Stomp) upgradeBoxes[4].SetActive(true);
            else upgradeBoxes[4].SetActive(false);
        }
    }

    public void QuitToTitle(InputAction.CallbackContext input)
    {
        if (inputLock) return;

        if (input.phase == InputActionPhase.Started)
        {
            GameObject.Find("FadeCanvas").GetComponent<Animator>().SetTrigger("FadeOut");
            StartCoroutine(WaitForFade());
        }
    }

    public void Unpause(InputAction.CallbackContext input)
    {
        if (inputLock) return;

        if (input.phase == InputActionPhase.Started)
        {
            PauseManager.Pause();
            GameObject.Find("MANAGER").GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        }
    }

    IEnumerator WaitForFade()
    {
        inputLock = true;
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(0);
        PauseManager.Pause();
    }
}
