using System;
using UnityEngine;
using UnityEngine.UI;


public class Crosshair : MonoBehaviour
{
    [SerializeField] private CrosshairManager m_crosshairManager;
    [SerializeField] private Canvas m_myCanvas;

    [SerializeField] Image m_centerImage;

    [SerializeField] Image m_rightInnerImage;
    [SerializeField] Image m_rightOutImage;

    [SerializeField] Image m_leftInnerImage;
    [SerializeField] Image m_leftOutImage;

    [SerializeField] Image m_upInnerImage;
    [SerializeField] Image m_upOutImage;

    [SerializeField] Image m_downInnerImage;
    [SerializeField] Image m_downOutImage;

    private void Awake()
    {
        m_crosshairManager.SetCrosshair(this);
    }

    public void ShowCrosshair(bool show)
    {
        m_myCanvas.enabled = show;
        m_centerImage.color = Color.red;
    }

    void ResetColors()
    {

        m_rightInnerImage.color = Color.white;
        m_rightOutImage.color = Color.white;

        m_leftInnerImage.color = Color.white;
        m_leftOutImage.color = Color.white;

        m_upInnerImage.color = Color.white;
        m_upOutImage.color = Color.white;

        m_downInnerImage.color = Color.white;
        m_downOutImage.color = Color.white;
    }


    public void ShowDragCrosshair(PlayerCubeRotation.DragDirection rotationDirection, float steps)
    {

        ResetColors();

        switch (rotationDirection)
        {
            case PlayerCubeRotation.DragDirection.Up:
                if(steps >= 1 && steps < 2)
                {
                    m_upInnerImage.color = Color.red;
                }
                else if(steps >= 2)
                {
                    m_upInnerImage.color = Color.red;
                    m_upOutImage.color = Color.red;
                }

                break;
            case PlayerCubeRotation.DragDirection.Down:
                if (steps >= 1 && steps < 2)
                {
                    m_downInnerImage.color = Color.red;
                }
                else if (steps >= 2)
                {
                    m_downInnerImage.color = Color.red;
                    m_downOutImage.color = Color.red;
                }

                break;
            case PlayerCubeRotation.DragDirection.Right:
                if (steps >= 1 && steps < 2)
                {
                    m_rightInnerImage.color = Color.red;
                }
                else if (steps >= 2)
                {
                    m_rightInnerImage.color = Color.red;
                    m_rightOutImage.color = Color.red;
                }

                break;
            case PlayerCubeRotation.DragDirection.Left:
                if (steps >= 1 && steps < 2)
                {
                    m_leftInnerImage.color = Color.red;
                }
                else if (steps >= 2)
                {
                    m_leftInnerImage.color = Color.red;
                    m_leftOutImage.color = Color.red;
                }
                break;
        }
    }
}
