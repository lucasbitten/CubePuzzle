using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    [SerializeField]
    Transform player = default;

    [SerializeField]
    float sensitivity = 5f;

    [SerializeField]
    float xRotation = 0f;

    [SerializeField]
    bool locked;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            locked = false;
        }

        if(locked){

            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation,-90f,90f);
            transform.localRotation = Quaternion.Euler(xRotation,0f,0f);

            player.Rotate(Vector3.up * mouseX);
         }
    }
}
