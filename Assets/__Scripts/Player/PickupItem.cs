using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    //TODO: corrigir colisoes entre itens
    //TODO: corrigir comprotamento enquanto esta sendo carregado


    [SerializeField] float m_maxDistanceToPickUp = 1;
    [SerializeField] bool m_canHold = true;
    [SerializeField] GameObject m_item;
    [SerializeField] GameObject m_tempParent;
    [SerializeField] bool m_isHolding = false;
    [SerializeField] LayerMask m_itemLayer;
    [SerializeField] float m_dropDistance = 1;

    [SerializeField] GameEvent_GameObject m_onItemPickedUpEvent;


    private Collider[] m_overlapResults = new Collider[10];
    Vector3 m_objectPos;
    float m_distance;

    Rigidbody m_itemRigibody;



    private void OnDrawGizmos() {
        
        Gizmos.DrawWireSphere(m_tempParent.transform.position,1);
    }


    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.G)) 
        {

            if (m_isHolding == false) 
            {
                PickUp();
            } else
            {
                Drop();
            }

        }


        if(m_isHolding)
        {
            m_distance = Vector3.Distance(m_item.transform.position, m_tempParent.transform.position);
            m_item.transform.SetParent(m_tempParent.transform);
            if (m_itemRigibody != null)
            {
                m_itemRigibody.velocity = Vector3.zero;
                m_itemRigibody.angularVelocity = Vector3.zero;
            }

            if(m_distance >= m_dropDistance)
            {
                Drop();
            }

        }

        
    }

    void PickUp(){

        if (m_isHolding == false) 
        {
            int numFound = Physics.OverlapSphereNonAlloc(m_tempParent.transform.position,1, m_overlapResults,m_itemLayer);

            if(numFound == 0)
            {
                return;
            }

            m_item = m_overlapResults[0].gameObject;
            for (int i = 0; i < numFound; i++) 
            {
                if (m_item.transform.position.y < m_overlapResults[i].transform.position.y)
                {
                    m_item = m_overlapResults[i].gameObject;
                }
                else
                {
                    if (Vector3.Distance(m_item.transform.position, transform.position) > Vector3.Distance(m_overlapResults[i].transform.position, transform.position))
                    {
                        m_item = m_overlapResults[i].gameObject;
                    }
                }
            }

            m_itemRigibody = m_item.GetComponent<Rigidbody>();
            m_distance = Vector3.Distance(m_item.transform.position, m_tempParent.transform.position);

            if (m_distance <= m_maxDistanceToPickUp) 
            {
                if(m_itemRigibody != null)
                {
                    m_itemRigibody.useGravity = false;

                    StartCoroutine(MoveItem(m_tempParent.transform.position));
                    m_itemRigibody.detectCollisions = true;
                }
            }
        }
    }

    public void Drop(){
        
        m_isHolding = false;
        var item = m_item.GetComponent<Item>();
        if (item != null)
        {
            item.DetachItem();
        }

        if (m_itemRigibody != null)
        {
            m_itemRigibody.useGravity = true;
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

        m_onItemPickedUpEvent.Raise(m_item.gameObject);
        m_isHolding = true;

    }
}
