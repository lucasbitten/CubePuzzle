using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody myRigidBody;

    [SerializeField]
    float moveSpeed = 5;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        ControlMovement();
    }

    private void ControlMovement()
    {
        float translation = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float straffe = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        transform.Translate(straffe, 0, translation);
    }
}
