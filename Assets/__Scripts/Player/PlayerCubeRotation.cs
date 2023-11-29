using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerCubeRotation : MonoBehaviour
{
    public enum DragDirection
    {
        None,
        Up,
        Down,
        Right,
        Left,
    }


    [SerializeField, Required] CubeManager m_cubeManager;
    [SerializeField, Required] CrosshairManager m_crosshairManager;
    [SerializeField, Required] GameEvent_Void m_onDragStartedEvent;
    [SerializeField, Required] GameEvent_Void m_onDragEndedEvent;
    [SerializeField, Required] Transform m_playerCamera;

    [SerializeField] LayerMask m_cubeFacesLayer;

    Vector3 m_initialDragPosition;
    DragDirection m_rotationDirection;
    MoveableFace m_selectedFace;
    void Update()
    {
        m_selectedFace = null;

        if(!m_cubeManager.CanRotate())
        {
            m_crosshairManager.UpdateIsDragEnableCrosshair(false);
            return;
        }


        Ray ray = new Ray(m_playerCamera.position, m_playerCamera.forward);
        RaycastHit hit;
        bool dragEnabled = false;
        if (Physics.Raycast(ray, out hit, 100f, m_cubeFacesLayer.value))
        {
            if (hit.collider.TryGetComponent(out m_selectedFace))
            {
                dragEnabled = m_selectedFace.CurrentFacePosition != MoveableFace.FacePosition.Top &&
                    m_selectedFace.CurrentFacePosition != MoveableFace.FacePosition.Bottom;

                m_crosshairManager.UpdateIsDragEnableCrosshair(dragEnabled);
            }
        }


        if (Input.GetMouseButtonDown(1) && dragEnabled)
        {
            if(!m_cubeManager.IsRotating() && !m_cubeManager.HasItemFalling()) 
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;

                m_crosshairManager.ShowCrosshair(true);
                m_initialDragPosition = Input.mousePosition;
                m_onDragStartedEvent.Raise();

                Debug.Log($"Hit Face {hit.collider.name}");
                if (dragEnabled)
                {
                    m_selectedFace.Selected();
                }

                m_crosshairManager.UpdateIsDragEnableCrosshair(dragEnabled);
            }
        }

        if(Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.Locked;

            m_crosshairManager.ShowCrosshair(false);
            m_onDragEndedEvent.Raise();
        }

        if(Input.GetMouseButton(1))
        {
            if(!dragEnabled)
            {
                return;
            }

            var dragDelta = Input.mousePosition - m_initialDragPosition;

            if(Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y) && Mathf.Abs(dragDelta.x) > m_crosshairManager.MouseDragThreshold)
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
                m_crosshairManager.SetRotation(m_rotationDirection, dragDelta.x, m_selectedFace);

            }
            else if(Mathf.Abs(dragDelta.y) > m_crosshairManager.MouseDragThreshold)
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

                m_crosshairManager.SetRotation(m_rotationDirection, dragDelta.y, m_selectedFace);
            }
            else
            {
                m_crosshairManager.SetRotation(DragDirection.None, 0, null);
                m_crosshairManager.ResetColors();
            }
        }
    }
}
