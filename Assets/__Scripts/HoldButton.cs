using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldButton : Button
{
    [SerializeField] bool m_lockItem;

    public override Item.ItemState OnAttach()
    {
        return m_lockItem ? Item.ItemState.ItemLocked : Item.ItemState.OnButton;
    }

    void OnTriggerExit(Collider other)
    {
        HandlePressing(other, false);
    }

    void OnTriggerEnter(Collider other)
    {
        HandlePressing(other, true);
    }

    protected override void HandlePressing(Collider collider, bool pressed)
    {
        if (m_interactable != null)
        {
            if (collider.CompareTag("Items") || collider.CompareTag("Player"))
            {
                m_pressed = pressed;
                m_animator.SetBool("Activated", pressed);
                if(pressed)
                {
                    m_interactable.OnActivated();
                }
                else
                {
                    m_interactable.OnDeactivated();
                }
                m_onInteractableChanged.Raise(m_interactableGO);

            }
        }
    }

}
