using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] GameEvent_Void m_onDragStarted;
    [SerializeField] GameEvent_Void m_onDragEnded;
    [SerializeField] Transform m_player = default;

    [SerializeField] float m_sensitivity = 5f;

    [SerializeField] float m_xRotation = 0f;

    [SerializeField] bool m_locked;

    bool m_dragging;

    private void Awake()
    {
        m_onDragStarted.EventListeners += OnDragStarted;
        m_onDragEnded.EventListeners += OnDragEnded;
    }

    private void OnDragEnded(Void obj)
    {
        m_dragging = false;
    }

    private void OnDragStarted(Void obj)
    {
        m_dragging = true;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.Escape)){
            m_locked = false;
            Cursor.lockState = CursorLockMode.None;
            m_dragging = false;
        }

        if(Input.GetMouseButtonDown(0))
        {
            m_locked = true;
            Cursor.lockState = CursorLockMode.Locked;

        }

        if (m_locked && !m_dragging)
        {

            float mouseX = Input.GetAxis("Mouse X") * m_sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * m_sensitivity * Time.deltaTime;

            m_xRotation -= mouseY;
            m_xRotation = Mathf.Clamp(m_xRotation,-90f,90f);
            transform.localRotation = Quaternion.Euler(m_xRotation,0f,0f);

            m_player.Rotate(Vector3.up * mouseX);
        }

    }
}
