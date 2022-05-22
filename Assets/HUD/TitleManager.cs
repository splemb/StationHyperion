using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void Begin(InputAction.CallbackContext input)
    {
        SceneManager.LoadScene(1);
    }

    public void EraseFile(InputAction.CallbackContext input)
    {
        SerializationManager.Erase("file1");
    }

    public void Quit(InputAction.CallbackContext input)
    {
        Application.Quit();
    }
}
