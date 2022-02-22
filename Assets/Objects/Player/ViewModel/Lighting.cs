using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    MeshRenderer meshRenderer;
    Material mat;

    bool unlit;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mat = meshRenderer.material;
        mat.EnableKeyword("_EMISSION");

        mat.SetTexture("_EmissionMap", mat.mainTexture);

        if (unlit) mat.SetColor("_EmissionColor", mat.color);
        else mat.SetColor("_EmissionColor", Color.black);
    }

    void SetUnlit(bool setUnlit)
    {
        Debug.Log("Unlit: " + setUnlit);

        if (setUnlit != unlit)
        {
            if (setUnlit) mat.SetColor("_EmissionColor", mat.color);
            else mat.SetColor("_EmissionColor", Color.black);

            unlit = setUnlit;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "EnvironmentLight" && !unlit) SetUnlit(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "EnvironmentLight" && unlit) SetUnlit(false);
    }
}
