using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    public enum ItemState
    {
        Invalid = -1,
        Falling,
        BeingCarried,
        OnObstacle,
        OnItem,
        OnBottom,
        OnItemHolder
    }


    //todo: fix item not attaching to correct face

    [SerializeField] Rigidbody m_myRigidbody;

    [SerializeField] GameEvent_Void m_onRotationStartedEvent;
    [SerializeField] GameEvent_Void m_onRotationEndedEvent;


    [SerializeField] float m_collisionRadius;
    [SerializeField] LayerMask m_objectsToAttachLayer;

    [SerializeField] Transform m_objectToAttach;
    [field: SerializeField] public ItemState ItemCurrentState { get; private set; } = ItemState.Invalid;

    bool m_lockMovement;

    private void Start()
    {
        Physics.IgnoreLayerCollision(9,10);
        CheckUnderneath();
    }

    private void OnEnable()
    {
        m_onRotationStartedEvent.EventListeners += OnRotationStarted;
        m_onRotationEndedEvent.EventListeners += OnRotationEnded;
    }

    private void OnDisable()
    {
        m_onRotationStartedEvent.EventListeners -= OnRotationStarted;
        m_onRotationEndedEvent.EventListeners -= OnRotationEnded;
    }

    private void OnRotationEnded(Void args)
    {
        UnlockMovement();
    }
    private void OnRotationStarted(Void args)
    {
        LockMovement();
    }


    public void SetState(ItemState state)
    {
        ItemCurrentState = state;
    }

    public void LockMovement()
    {
        m_myRigidbody.constraints = RigidbodyConstraints.FreezeAll;    
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;        
        //Gizmos.DrawWireSphere(transform.position, m_collisionRadius);
        var ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * 0.5f, Color.yellow);
    }

    public void UnlockMovement()
    {
        if(ItemCurrentState == ItemState.OnItemHolder)
        {
            return;
        }

        if(m_objectToAttach != null && m_objectToAttach.TryGetComponent(out MoveableFace face))
        {
            if(face.CurrentFacePosition != MoveableFace.FacePosition.Bottom)
            {
                DetachItem();
            }
        }
        else
        {
            CheckUnderneath();
        }
    }

    private void Update()
    {
        if (ItemCurrentState == ItemState.OnItemHolder)
        {
            return;
        }

        CheckWhereItemHasFallen();
    }

    public bool CheckCollision()
    {
        var ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.5f, m_objectsToAttachLayer))
        {
            var hitGameObject = hitInfo.collider.gameObject;
            if (hitGameObject.TryGetComponent(out MoveableFace face))
            {
                if (face.CurrentFacePosition == MoveableFace.FacePosition.Bottom)
                {
                    if (hitGameObject.transform != transform)
                    {
                        AttachToObject(hitGameObject.transform, ItemState.OnBottom);
                        return true;
                    }
                }
            }
            else if (hitGameObject.CompareTag("Obstacles"))
            {
                AttachToObject(hitGameObject.transform, ItemState.OnObstacle);
                return true;

            }
            else if (hitGameObject.gameObject.CompareTag("Items"))
            {
                AttachToObject(hitGameObject.transform, ItemState.OnItem);
                return true;
            }
        }
        return false;
    }

    public void CheckUnderneath()
    {
        if(!CheckCollision())
        {
            DetachItem();
        }
    }

    private void CheckWhereItemHasFallen()
    {
        if (ItemCurrentState == ItemState.Falling)
        {
            CheckCollision();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(m_objectToAttach != null)
        {
            if (other.gameObject == m_objectToAttach.gameObject)
            {
                m_myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                var vec = transform.eulerAngles;
                vec.x = Mathf.Round(vec.x / 90) * 90;
                vec.y = Mathf.Round(vec.y / 90) * 90;
                vec.z = Mathf.Round(vec.z / 90) * 90;
                transform.eulerAngles = vec;

                transform.SetParent(m_objectToAttach);
                if (m_lockMovement)
                {
                    LockMovement();
                }
            }
        }

        if(ItemCurrentState == ItemState.BeingCarried)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                // pickupItem.Drop();
            }
        }
    }

    public void DetachItem()
    {
        ItemCurrentState = ItemState.Falling;
        m_myRigidbody.constraints = RigidbodyConstraints.None;
        transform.SetParent(transform.root);
        m_objectToAttach = null;
    }

    public void AttachToObject(Transform parent, ItemState itemState, bool lockConstraints = true)
    {
        ItemCurrentState = itemState;
        m_objectToAttach = parent;
        m_lockMovement = lockConstraints;
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Goal")
        {
            Debug.Log("Level Completo");
        }
    }


}
