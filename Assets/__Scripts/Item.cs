using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    //todo: fix item not attaching to correct face

    [SerializeField] Rigidbody m_myRigidbody;

    public bool m_falling;
    public bool m_beingCarried;

    [SerializeField] GameEvent_Void m_onRotationEnded;
    [SerializeField] GameEvent_Void m_onRotationStarted;


    [SerializeField] float m_collisionRadius;
    public LayerMask m_objectsToAttachLayer;

    [SerializeField] Transform m_currentParent;


    private void Start()
    {
        Physics.IgnoreLayerCollision(9,10);

        var hitColliders = Physics.OverlapSphere(transform.position, m_collisionRadius, m_objectsToAttachLayer.value);
        foreach (var hit in hitColliders)
        {
            if(hit.transform != transform)
            {
                AttachToObject(hit.transform);
            }
        }
    }

    private void OnEnable()
    {
        m_onRotationStarted.EventListeners += OnRotationStarted;
        m_onRotationEnded.EventListeners += OnRotationEnded;
    }

    private void OnDisable()
    {
        m_onRotationStarted.EventListeners -= OnRotationStarted;
        m_onRotationEnded.EventListeners -= OnRotationEnded;
    }

    private void OnRotationEnded(Void args)
    {
        UnlockMovement();
    }
    private void OnRotationStarted(Void args)
    {
        LockMovement();
    }

    public void LockMovement()
    {
        m_myRigidbody.constraints = RigidbodyConstraints.FreezeAll;   
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;        
        Gizmos.DrawWireSphere(transform.position, m_collisionRadius);
    }

    public void UnlockMovement()
    {
        if(m_currentParent != null && m_currentParent.TryGetComponent(out MoveableFace face))
        {
            if(face.CurrentFacePosition != MoveableFace.FacePosition.Bottom)
            {
                DetachItem();
                m_falling = true;
            }
        }
        else if(m_currentParent.gameObject.layer == LayerMask.NameToLayer("Obstacle") && m_currentParent.transform.position.y > transform.position.y)
        {
            DetachItem();
            m_falling = true;
        }
    }

    private void Update()
    {
        if (m_falling)
        {
            var hitColliders = Physics.OverlapSphere(transform.position, m_collisionRadius, m_objectsToAttachLayer.value);
            foreach (var hit in hitColliders)
            {
                if (hit.transform == transform || hit.transform == m_currentParent)
                {
                    continue;
                }

                if (hit.gameObject.CompareTag("Faces"))
                {
                    var face = hit.gameObject.GetComponent<MoveableFace>();
                    if (face && face.CurrentFacePosition == MoveableFace.FacePosition.Bottom)
                    {
                        AttachToObject(hit.transform);
                    }
                }
                else if (hit.gameObject.CompareTag("Items") || hit.gameObject.CompareTag("Obstacles"))
                {
                    AttachToObject(hit.transform);
                }



            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(m_beingCarried)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                // pickupItem.Drop();
            }
        }
    }

    public void DetachItem()
    {
        m_myRigidbody.constraints = RigidbodyConstraints.None;
        transform.SetParent(transform.root);
    }

    public void AttachToObject(Transform parent, bool lockConstraints = true)
    {
        m_falling = false;

        m_myRigidbody.constraints = RigidbodyConstraints.FreezeRotation; 

        var vec = transform.eulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        transform.eulerAngles = vec;

        m_currentParent = parent;
        transform.SetParent(parent);
        if(lockConstraints)
        {
            LockMovement();
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Goal")
        {
            Debug.Log("Level Completo");
        }
    }


}
