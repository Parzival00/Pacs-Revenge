using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public PlayerInput playerInput;
    public float mouseSens = 50;
    float xRotation = 0f;

    Transform playerBody;

    private void Awake()
    {
        playerBody = transform.parent;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 mouseLook;

        //gets input from the mouse
        mouseLook = playerInput.actions["Look"].ReadValue<Vector2>();
        float mouseX = mouseLook.x * mouseSens * Time.deltaTime;
        float mouseY = mouseLook.y * mouseSens * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90, 90);

        //applies mouse movement
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
