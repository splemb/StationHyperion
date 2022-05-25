using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDemo : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().saveData.respawnPoint = "NEWGAME";
        SerializationManager.Save(FileNameTracker.FileName, GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>().saveData);
        QuitToTitle();
    }

    public void QuitToTitle()
    {
        
        StartCoroutine(WaitForFade());
    }

    IEnumerator WaitForFade()
    {
        yield return new WaitForSecondsRealtime(5f);
        GameObject.Find("FadeCanvas").GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene(0);
    }
}
