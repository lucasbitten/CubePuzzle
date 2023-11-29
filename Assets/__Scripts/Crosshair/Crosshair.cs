using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;


public class Crosshair : MonoBehaviour
{
    [SerializeField, Required] private CrosshairManager m_crosshairManager;
    [SerializeField, Required] private Canvas m_arrowsCanvas;

    [SerializeField, Required] Image m_centerImage;
    [SerializeField, Required] Sprite m_dragEnabledSprite;
    [SerializeField, Required] Sprite m_dragDisabledSprite;

    [SerializeField, Required] Image m_rightInnerImage;
    [SerializeField, Required] Image m_rightOutImage;

    [SerializeField, Required] Image m_leftInnerImage;
    [SerializeField, Required] Image m_leftOutImage;

    [SerializeField, Required] Image m_upInnerImage;
    [SerializeField, Required] Image m_upOutImage;

    [SerializeField, Required] Image m_downInnerImage;
    [SerializeField, Required] Image m_downOutImage;

    private void Awake()
    {
        m_crosshairManager.SetCrosshair(this);
    }

    public void ShowCrosshair(bool show)
    {
        m_arrowsCanvas.enabled = show;
        m_centerImage.color = Color.red;
        ResetColors();
    }

    public void ResetColors()
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


    public void ShowDragCrosshair(PlayerCubeRotation.DragDirection rotationDirection, int steps)
    {
        Debug.Log($"Steps {steps}");
        ResetColors();

        switch (rotationDirection)
        {
            case PlayerCubeRotation.DragDirection.Up:
                if(steps == 1)
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
                if (steps == 1)
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
                if (steps == 1)
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
                if (steps == 1)
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

    public void UpdateIsDragEnableCrosshair(bool enabled)
    {
        if (enabled)
        {
            m_centerImage.sprite = m_dragEnabledSprite;
        }
        else
        {
            m_centerImage.sprite = m_dragDisabledSprite;
        }
    }
}
