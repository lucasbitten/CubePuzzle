using System;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/Crosshair Maanager")]
public class CrosshairManager : ScriptableObject
{

    Crosshair m_crosshair;
    [SerializeField] float m_mouseDragStep = 5f;


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

    public void SetRotation(PlayerCubeRotation.DragDirection rotationDirection, float dragDistance)
    {
        var steps = Mathf.Abs(dragDistance / m_mouseDragStep);
        m_crosshair.ShowDragCrosshair(rotationDirection, steps);
    }
}
