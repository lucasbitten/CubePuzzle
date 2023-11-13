using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubeRotation : MonoBehaviour
{
    public enum DragDirection
    {
        Up,
        Down,
        Right,
        Left,
    }

    [SerializeField] LayerMask m_cubeFacesLayer;


    [SerializeField] CrosshairManager m_crosshairManager;
    [SerializeField] GameEvent_Void m_onDragStarted;
    [SerializeField] GameEvent_Void m_onDragEnded;
    [SerializeField] Transform m_playerCamera;


    Vector3 m_initialDragPosition;
    DragDirection m_rotationDirection;
    MoveableFace m_selectedFace;
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;

            m_crosshairManager.ShowCrosshair(true);
            m_initialDragPosition = Input.mousePosition;
            m_onDragStarted.Raise();

            Ray ray = new Ray(m_playerCamera.position, m_playerCamera.forward);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100f, m_cubeFacesLayer.value))
            {
                if(hit.collider.TryGetComponent(out m_selectedFace))
                {
                    Debug.Log($"Hit Face {hit.collider.name}");
                    if(m_selectedFace.CurrentFacePosition == MoveableFace.FacePosition.Top ||
                        m_selectedFace.CurrentFacePosition == MoveableFace.FacePosition.Bottom)
                    {
                        m_selectedFace = null;
                    }
                    else
                    {
                        m_selectedFace.Selected();
                    }
                }
            }

        }

        if(Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.Locked;

            m_crosshairManager.ShowCrosshair(false);
            m_onDragEnded.Raise();

            //todo: handle rotation

        }

        if(Input.GetMouseButton(1))
        {
            if (m_selectedFace == null)
            {
                return;
            }
            var dragDelta = Input.mousePosition - m_initialDragPosition;

            if(Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
            {
                //Dragging on X axis
                if(dragDelta.x >= 0)
                {
                    m_rotationDirection = DragDirection.Right;
                }
                else
                {
                    m_rotationDirection = DragDirection.Left;
                }

                //todo: calcular qual direçao é em relaçao a face selecionada
                m_crosshairManager.SetRotation(m_rotationDirection, dragDelta.x, m_selectedFace);

            }
            else
            {
                //Dragging on Y axis
                if (dragDelta.y >= 0)
                {
                    m_rotationDirection = DragDirection.Up;
                }
                else
                {
                    m_rotationDirection = DragDirection.Down;
                }

                //todo: calcular qual direçao é em relaçao a face selecionada
                m_crosshairManager.SetRotation(m_rotationDirection, dragDelta.y, m_selectedFace);

            }


        }


    }
}
