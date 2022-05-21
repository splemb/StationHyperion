using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textMesh;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void DisplayMessage(string msg="")
    {
        Debug.Log(msg);
        textMesh.text = msg;
        animator.SetTrigger("ShowMessage");
    }
}
