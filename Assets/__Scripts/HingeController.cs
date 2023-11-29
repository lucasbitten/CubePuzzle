using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeController : MonoBehaviour, IInteractable, IAttachable
{
    [SerializeField, Required] GameObject m_hingedObject;
    [SerializeField, Required] Rigidbody m_rigidbody;

    [SerializeField] float m_openAngle = 90;
    [SerializeField] float m_closeAngle = 0;
    [SerializeField] float m_speed = 5;

    public bool IsActive { get => m_isActive; set => m_isActive = value; }

    [SerializeField] bool m_isActive;

    Vector3 m_eulerAngleVelocity;

    private void Awake()
    {
        m_eulerAngleVelocity = new Vector3(0, m_speed, 0);
    }

    public void OnActivated()
    {
        m_isActive = true;
    }

    public void OnDeactivated()
    {
        m_isActive = false;
    }

    public Item.ItemState OnAttach()
    {
        return Item.ItemState.OnObstacle;
    }

    private void FixedUpdate()
    {
        float targetAngle = m_isActive ? m_openAngle : m_closeAngle;
        float currentAngle = m_rigidbody.transform.localEulerAngles.y;

        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
        float torque = angleDifference * m_speed;

        m_rigidbody.AddTorque(m_rigidbody.transform.up * torque);

    }

}
