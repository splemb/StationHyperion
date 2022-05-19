using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    GameObject GAME;

    private void Start()
    {
        GAME = GameObject.Find("GAME");
    }

    public void Pause(InputAction.CallbackContext input)
    {
        if (input.phase == InputActionPhase.Started)
        {
            GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
            GAME.SetActive(false);
        }
    }

    public void Unpause(InputAction.CallbackContext input)
    {
        GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        GAME.SetActive(true);
    }
}
