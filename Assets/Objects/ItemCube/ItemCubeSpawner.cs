using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCubeSpawner : MonoBehaviour
{
    bool checkedItems = false;
    [SerializeField] ItemCubeInteraction itemCube;

    private void OnEnable()
    {
        checkedItems = false;
        itemCube.gameObject.SetActive(true);
    }

    private void Update()
    {
        PlayerBehaviour player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();

        if (player.saveData != null && !checkedItems)
        {
            checkedItems = true;

            switch (itemCube.item)
            {
                case ItemCubeInteraction.Items.Grapple:
                    if (player.saveData.GrapplingHook) itemCube.gameObject.SetActive(false);
                    break;
                case ItemCubeInteraction.Items.Slide:
                    if (player.saveData.Slide) itemCube.gameObject.SetActive(false);
                    break;
                case ItemCubeInteraction.Items.Dash:
                    if (player.saveData.Dash) itemCube.gameObject.SetActive(false);
                    break;
                case ItemCubeInteraction.Items.Stomp:
                    if (player.saveData.Stomp) itemCube.gameObject.SetActive(false);
                    break;
                case ItemCubeInteraction.Items.WallJump:
                    if (player.saveData.WallJump) itemCube.gameObject.SetActive(false);
                    break;
            }
        }
    }
}
