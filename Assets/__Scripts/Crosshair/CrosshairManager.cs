using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static CubeMovementController;

[CreateAssetMenu(menuName = "Managers/Crosshair Maanager")]
public class CrosshairManager : ScriptableObject
{
    [SerializeField] CubeManager m_cubeManager;
    [SerializeField] float m_mouseDragStep = 5f;
    [field: SerializeField] public float MouseDragThreshold { get; private set; } = 30f;

    Crosshair m_crosshair;


    public void SetCrosshair(Crosshair crosshair)
    {
        m_crosshair = crosshair;
    }

    public void ShowCrosshair(bool show)
    {
        if (m_crosshair != null)
        {
            m_crosshair.ShowCrosshair(show);
        }
    }

    public void ResetColors()
    {
        if (m_crosshair != null)
        {
            m_crosshair.ResetColors();
        }
    }


    public void UpdateIsDragEnableCrosshair(bool dragEnabled)
    {
        m_crosshair.UpdateIsDragEnableCrosshair(dragEnabled);
    }

    public void SetRotation(PlayerCubeRotation.DragDirection rotationDirection, float dragDistance, MoveableFace selectedFace)
    {

        int steps = Mathf.RoundToInt(Mathf.Abs(dragDistance / m_mouseDragStep));
        if (steps > 2) 
        { 
            steps = 2;
        }


        if (steps == 0)
        {
            m_cubeManager.SetRotationInfo(null);
            return;
        }

        float angle = 90 * steps;
        RotationType rotationType = RotationType.Rotate_X;
        List<MoveableFace> faces = new List<MoveableFace>();

        switch (selectedFace.CurrentFacePosition)
        {
            case MoveableFace.FacePosition.Right:
                switch (rotationDirection)
                {
                    case PlayerCubeRotation.DragDirection.Up:
                        rotationType = RotationType.Rotate_Y;
                        break;
                    case PlayerCubeRotation.DragDirection.Down:
                        rotationType = RotationType.Rotate_Y;
                        angle = -angle;
                        break;
                    case PlayerCubeRotation.DragDirection.Right:
                        rotationType = RotationType.Rotate_X;
                        break;
                    case PlayerCubeRotation.DragDirection.Left:
                        rotationType = RotationType.Rotate_X;
                        angle = -angle;
                        break;
                    default:
                        break;
                }
                break;
            case MoveableFace.FacePosition.Left:

                switch (rotationDirection)
                {
                    case PlayerCubeRotation.DragDirection.Up:
                        rotationType = RotationType.Rotate_Y;
                        angle = -angle;
                        break;
                    case PlayerCubeRotation.DragDirection.Down:
                        rotationType = RotationType.Rotate_Y;
                        break;
                    case PlayerCubeRotation.DragDirection.Right:
                        rotationType = RotationType.Rotate_X;
                        break;
                    case PlayerCubeRotation.DragDirection.Left:
                        rotationType = RotationType.Rotate_X;
                        angle = -angle;
                        break;
                    default:
                        break;
                }

                break;
            case MoveableFace.FacePosition.Forward:
                switch (rotationDirection)
                {
                    case PlayerCubeRotation.DragDirection.Up:
                        rotationType = RotationType.Rotate_Z;
                        angle = -angle;
                        break;
                    case PlayerCubeRotation.DragDirection.Down:
                        rotationType = RotationType.Rotate_Z;
                        break;
                    case PlayerCubeRotation.DragDirection.Right:
                        rotationType = RotationType.Rotate_X;
                        break;
                    case PlayerCubeRotation.DragDirection.Left:
                        rotationType = RotationType.Rotate_X;
                        angle = -angle;
                        break;
                    default:
                        break;
                }

                break;
            case MoveableFace.FacePosition.Back:
                switch (rotationDirection)
                {
                    case PlayerCubeRotation.DragDirection.Up:
                        rotationType = RotationType.Rotate_Z;
                        break;
                    case PlayerCubeRotation.DragDirection.Down:
                        angle = -angle;
                        rotationType = RotationType.Rotate_Z;
                        break;
                    case PlayerCubeRotation.DragDirection.Right:
                        rotationType = RotationType.Rotate_X;
                        break;
                    case PlayerCubeRotation.DragDirection.Left:
                        rotationType = RotationType.Rotate_X;
                        angle = -angle;
                        break;
                    default:
                        break;
                }

                break;
            default:
                break;
        }



        m_crosshair.UpdateIsDragEnableCrosshair(true);
        m_crosshair.ShowDragCrosshair(rotationDirection, steps);
        Debug.Log($"Angle: {angle}, RotationType: {rotationType}");
        m_cubeManager.SetRotationInfo(new RotationInfo(angle, rotationType, faces));
    }
}
