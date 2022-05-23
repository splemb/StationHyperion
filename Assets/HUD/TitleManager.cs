using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    string[] fileNames = new string[] { "file1", "file2", "file3" };
    int fileIndex = 0;
    [SerializeField] TMPro.TextMeshProUGUI fileNameDisplay;
    [SerializeField] AudioSource selectAudio;
    [SerializeField] AudioSource eraseAudio;
    [SerializeField] AudioSource startAudio;
    bool inputLock = false;

    private void Start()
    {

        switch (FileNameTracker.FileName)
        {
            case "file1":
                fileIndex = 0;
                break;
            case "file2":
                fileIndex = 1;
                break;
            case "file3":
                fileIndex = 2;
                break;
        }
        Cursor.visible = false;
        UpdateFileName();
    }

    public void Begin(InputAction.CallbackContext input)
    {
        if (inputLock) return;
        if (input.phase == InputActionPhase.Started)
        {
            startAudio.Play();
            inputLock = true;
            StartCoroutine(WaitForFade());
            GameObject.Find("FadeCanvas").GetComponent<Animator>().SetTrigger("FadeOut");
        }
    }

    public void EraseFile(InputAction.CallbackContext input)
    {
        if (inputLock) return;
        if (input.phase == InputActionPhase.Started)
        {
            eraseAudio.Play();
            SerializationManager.Erase(FileNameTracker.FileName);
        }
    }

    public void Quit(InputAction.CallbackContext input)
    {
        if (inputLock) return;
        if (input.phase == InputActionPhase.Started)
        {
            inputLock = true;
            GameObject.Find("FadeCanvas").GetComponent<Animator>().SetTrigger("FadeOut");
            StartCoroutine(WaitForFadeExit());
        }
    }

    public void NextFile(InputAction.CallbackContext input)
    {
        if (inputLock) return;
        if (input.phase == InputActionPhase.Started)
        {
            fileIndex++;
            if (fileIndex >= fileNames.Length) fileIndex = 0;
            selectAudio.Play();
            UpdateFileName();
        }
    }

    public void PrevFile(InputAction.CallbackContext input)
    {
        if (inputLock) return;
        if (input.phase == InputActionPhase.Started)
        {
            fileIndex--;
            if (fileIndex < 0) fileIndex = fileNames.Length - 1;
            selectAudio.Play();
            UpdateFileName();
        }
    }

    void UpdateFileName()
    {
        FileNameTracker.SetFileName(fileNames[fileIndex]);
        Debug.Log(FileNameTracker.FileName);
        
        switch (FileNameTracker.FileName)
        {
            case "file1":
                fileNameDisplay.text = "File 1";
                break;
            case "file2":
                fileNameDisplay.text = "File 2";
                break;
            case "file3":
                fileNameDisplay.text = "File 3";
                break;
        } 
    }

    IEnumerator WaitForFade()
    {
        inputLock = true;
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene(1);
    }

    IEnumerator WaitForFadeExit()
    {
        inputLock = true;
        yield return new WaitForSecondsRealtime(2f);
        Application.Quit();
    }
}
