using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeController : MonoBehaviour, IInteractable, IAttachable
{
    [SerializeField, Required] GameObject m_hingedObject;

    [SerializeField] float m_openAngle = 90;
    [SerializeField] float m_closeAngle = 0;
    [SerializeField] float m_speed = 5;
    [SerializeField] bool isExtended;

    public bool IsActive { get => isExtended; set => isExtended = value; }


    Rigidbody m_rigidbody;


    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    public void OnActivated()
    {
        isExtended = true;
    }

    public void OnDeactivated()
    {
        isExtended = false;
    }

    public Item.ItemState OnAttach()
    {
        return Item.ItemState.OnObstacle;
    }

    private void FixedUpdate()
    {
        float targetAngle = isExtended ? m_openAngle : m_closeAngle;
        float currentAngle = m_rigidbody.transform.localEulerAngles.y;

        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
        float torque = angleDifference * m_speed;

        m_rigidbody.AddTorque(m_rigidbody.transform.up * torque);

    }

}
