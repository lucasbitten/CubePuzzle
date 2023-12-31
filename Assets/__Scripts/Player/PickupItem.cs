﻿using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    //TODO: corrigir colisoes entre itens
    //TODO: corrigir comprotamento enquanto esta sendo carregado


    [SerializeField, Required] Transform m_pickupParent;
    [SerializeField, Required] GameEvent_GameObject m_onItemPickedUpEvent;

    [SerializeField] float m_maxDistanceToPickUp = 1;
    [SerializeField] float m_dropDistance = 1;
    [SerializeField] float m_currentItemDrag = 10f;
    [SerializeField] LayerMask m_itemLayer;

    GameObject m_item;
    public bool IsHoldingItem { get; private set; }
    float m_distance;

    Rigidbody m_itemRigibody;
    private float m_previousDrag;

    private void OnDrawGizmos() {
        
        Gizmos.DrawWireSphere(m_pickupParent.transform.position,1);
    }


    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.G) /*|| Input.GetMouseButtonDown(0)*/) 
        {

            if (IsHoldingItem == false) 
            {
                PickUp();
            } else
            {
                Drop();
            }

        }


        if(IsHoldingItem)
        {
            m_distance = Vector3.Distance(m_item.transform.position, m_pickupParent.transform.position);
            m_item.transform.SetParent(m_pickupParent.transform);
            if (m_itemRigibody != null)
            {
                m_itemRigibody.velocity = Vector3.zero;
                m_itemRigibody.angularVelocity = Vector3.zero;
            }

            if (m_distance >= m_dropDistance)
            {
                Drop();
            }

        }

        
    }

    void PickUp(){

        if (IsHoldingItem == false) 
        {
            RaycastHit hit;
            if(Physics.Raycast(m_pickupParent.position, m_pickupParent.TransformDirection(Vector3.forward), out hit, m_maxDistanceToPickUp,  m_itemLayer))
            {
                m_item = hit.collider.gameObject;  

                m_itemRigibody = m_item.GetComponent<Rigidbody>();
                m_distance = Vector3.Distance(m_item.transform.position, m_pickupParent.transform.position);

                if (m_distance <= m_maxDistanceToPickUp)
                {
                    if (m_itemRigibody != null)
                    {
                        m_itemRigibody.useGravity = false;
                        var item = m_item.GetComponent<Item>();
                        if (item != null)
                        {
                            item.SetState(Item.ItemState.BeingCarried);
                        }
                        StartCoroutine(MoveItem(m_pickupParent.transform.position));

                    }
                }
            }
        }
    }

    public void Drop(){
        
        IsHoldingItem = false;
        var item = m_item.GetComponent<Item>();
        if (item != null)
        {
            item.DetachItem();
        }

        if (m_itemRigibody != null)
        {
            m_itemRigibody.useGravity = true;
            m_itemRigibody.drag = m_previousDrag;
        }

        m_item = null;
    }

    IEnumerator MoveItem(Vector3 position){
        float elapsedTime = 0;

        while (Vector3.Distance(m_item.transform.position, position) > 0.4f )
        {
            elapsedTime += Time.deltaTime;

            m_item.transform.position = Vector3.Slerp(m_item.transform.position, position, elapsedTime/2);
            yield return null;

        }

        m_previousDrag = m_itemRigibody.drag;
        m_itemRigibody.drag = m_currentItemDrag;
        m_itemRigibody.detectCollisions = true;
        m_onItemPickedUpEvent.Raise(m_item.gameObject);
        IsHoldingItem = true;

    }
}
