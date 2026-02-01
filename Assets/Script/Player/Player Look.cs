using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSens = 800f;
    public Transform orientation;
    public Transform playerBody;  

    public float xRotation;
    public float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSens;

        yRotation += mouseX; 
        xRotation -= mouseY; 
        xRotation = Mathf.Clamp(xRotation, -75f, 75f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        playerBody.rotation = Quaternion.Euler(0, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}