using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] GameObject m_interactableGO;
    IInteractable m_interactable;
    [SerializeField] bool m_pressed;

    private void Awake()
    {
        m_interactable = m_interactableGO.GetComponent<IInteractable>();
        Debug.Assert(m_interactable != null, "Button doesn't have an interactable object assigned", gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if(m_interactable != null)
        {
            if (other.CompareTag("Items") || other.CompareTag("Player"))
            {
                m_pressed = true;
                m_interactable.OnActivated();
            }
        }

    }
    void OnTriggerExit(Collider other)
    {
        if (m_interactable != null)
        {
            if (other.CompareTag("Items") || other.CompareTag("Player"))
            {
                m_pressed = false;
                m_interactable.OnDeactivated();
            }
        }
    }

}
