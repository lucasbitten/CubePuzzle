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
        OnBottom
    }


    //todo: fix item not attaching to correct face

    [SerializeField] Rigidbody m_myRigidbody;

    [SerializeField] GameEvent_Void m_onRotationEnded;
    [SerializeField] GameEvent_Void m_onRotationStarted;


    [SerializeField] float m_collisionRadius;
    [SerializeField] LayerMask m_objectsToAttachLayer;

    [SerializeField] Transform m_currentParent;
    [field: SerializeField] public ItemState ItemCurrentState { get; private set; } = ItemState.Invalid;

    private void Start()
    {
        Physics.IgnoreLayerCollision(9,10);
        CheckUnderneath();
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
                ItemCurrentState = ItemState.Falling;
            }
        }
        else
        {
            CheckUnderneath();
        }
    }

    private void Update()
    {
        CheckWhereItemHasFallen();
    }

    public void CheckUnderneath()
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
                    }
                }
            }
            else if (hitGameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                AttachToObject(hitGameObject.transform, ItemState.OnObstacle);
            }
        }
        else
        {
            DetachItem();
            ItemCurrentState = ItemState.Falling;
        }
    }

    private void CheckWhereItemHasFallen()
    {
        if (ItemCurrentState == ItemState.Falling)
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
                        AttachToObject(hit.transform, ItemState.OnBottom);
                    }
                }
                else if (hit.gameObject.CompareTag("Items"))
                {
                    AttachToObject(hit.transform, ItemState.OnItem);

                }
                else if (hit.gameObject.CompareTag("Obstacles"))
                {
                    AttachToObject(hit.transform, ItemState.OnObstacle);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
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
        m_myRigidbody.constraints = RigidbodyConstraints.None;
        transform.SetParent(transform.root);
    }

    public void AttachToObject(Transform parent, ItemState itemState, bool lockConstraints = true)
    {
        ItemCurrentState = itemState;
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
