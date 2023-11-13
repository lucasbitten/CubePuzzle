using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] float m_moveSpeed = 5;


    void Update()
    {
        ControlMovement();
    }

    private void ControlMovement()
    {
        float translation = Input.GetAxis("Vertical") * m_moveSpeed * Time.deltaTime;
        float straffe = Input.GetAxis("Horizontal") * m_moveSpeed * Time.deltaTime;

        transform.Translate(straffe, 0, translation);
    }
}
