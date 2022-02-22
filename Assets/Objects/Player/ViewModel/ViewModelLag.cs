using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ViewModelLag : MonoBehaviour
{
    float camDeltaX = 0.0f;
    float camDeltaY = 0.0f;

    [Header("Sway Settings")]
    [SerializeField] float smooth;
    [SerializeField] float swayMultiplier;

    public void MouseLook(InputAction.CallbackContext input)
    {
        camDeltaY = input.ReadValue<Vector2>().y * swayMultiplier;
        camDeltaX = input.ReadValue<Vector2>().x * swayMultiplier;
    }

    private void Update()
    {
        Quaternion rotationX = Quaternion.AngleAxis(-camDeltaY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(-camDeltaX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
