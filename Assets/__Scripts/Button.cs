using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

public class Button : MonoBehaviour, IAttachable
{
    [SerializeField, RequiredIn(PrefabKind.InstanceInScene)] protected GameObject m_interactableGO;
    [SerializeField, Required] protected GameEvent_GameObject m_onInteractableChanged;
    [SerializeField] protected bool m_pressed;
    [SerializeField] protected Animator m_animator;


    protected IInteractable m_interactable;

    public virtual Item.ItemState OnAttach()
    {
        return Item.ItemState.OnButton;
    }

    private void Awake()
    {
        m_interactable = m_interactableGO.GetComponent<IInteractable>();

        if(m_interactable == null )
        {
            m_interactable = m_interactableGO.GetComponentInChildren<IInteractable>();
        }

        Debug.Assert(m_interactable != null, "Button doesn't have an interactable object assigned", gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        HandlePressing(other, true);
    }

    protected virtual void HandlePressing(Collider collider, bool pressed)
    {
        if (m_interactable != null)
        {
            if (collider.CompareTag("Items") || collider.CompareTag("Player"))
            {
                m_pressed = !m_pressed;
                if (m_pressed)
                {
                    m_interactable.OnActivated();
                }
                else
                {
                    m_interactable.OnDeactivated();
                }
                m_onInteractableChanged.Raise(m_interactableGO);
                m_animator.SetBool("Activated", m_pressed);
            }
        }
    }


}
