using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{

    [SerializeField] Rigidbody m_myRigidbody;

    public bool m_falling;
    public bool m_locked;
    public bool m_beingCarried;

    [SerializeField] GameEvent_Void m_onRotationEnded;
    [SerializeField] GameEvent_Void m_onRotationStarted;

    public LayerMask m_raycastLayer;
    
    private void Start()
    {
        Physics.IgnoreLayerCollision(9,10);
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
        Ray ray = new Ray(transform.position, Vector3.down);
        Gizmos.DrawRay(ray);
    }

    public void UnlockMovement()
    {
        ClearConstraints();
        Ray ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(ray, out RaycastHit hit, 0.5f, m_raycastLayer);
        
        if(hit.collider)
        {
            if (hit.collider.CompareTag("Obstacles") || hit.collider.CompareTag("Faces"))
            {
                m_falling = false;
                Debug.LogWarning("Not Falling");
            }
            else if (hit.collider.CompareTag("Items"))
            {
                //Check if item is above other item
                Physics.Raycast(hit.collider.transform.position, Vector3.down, out RaycastHit itemHit, 0.5f, m_raycastLayer);
                if (itemHit.collider && (itemHit.collider.CompareTag("Obstacles") || itemHit.collider.CompareTag("Faces")))
                {
                    m_falling = false;
                    Debug.LogWarning("Not Falling too");

                }
                else
                {
                    m_falling = true;
                    Debug.LogWarning("Faaaalling");

                }
            }
        }
        else
        {
            m_falling = true;
            Debug.LogWarning("Falling");

        }

        m_locked = false;        
    }

    private void OnCollisionEnter(Collision other)
    {
        if(m_falling)
        {
            if(other.gameObject.CompareTag("Items"))
            {   
                if (other.transform.parent)
                {
                    AttachToObject(other.transform.parent.transform);
                } 
                else
                {
                    //transform.parent = cube.transform;
                }
                m_locked = false;
                m_falling = false;
            } 

            m_myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            var vec = transform.eulerAngles;
            vec.x = Mathf.Round(vec.x / 90) * 90;
            vec.y = Mathf.Round(vec.y / 90) * 90;
            vec.z = Mathf.Round(vec.z / 90) * 90;
            transform.eulerAngles = vec;
        }
        else
        {
            if (other.gameObject.CompareTag("Items"))
            {
                if (transform.position.y > other.transform.position.y)
                {
                    AttachToObject(other.transform.parent);
                }
            }
        }

        if(m_beingCarried)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                // pickupItem.Drop();
            }
        }

        if(other.gameObject.CompareTag("Faces"))
        {
            var face = other.gameObject.GetComponent<MoveableFace>();

            if (face && face.CurrentFacePosition == MoveableFace.FacePosition.Bottom)
            {
                AttachToObject(other.transform);
                m_falling = false;
                m_locked = false;
            }
        }
    }

    public void ClearConstraints()
    {
        m_myRigidbody.constraints = RigidbodyConstraints.None;
    }

    public void AttachToObject(Transform transform, bool lockConstraints = true)
    {
        transform.SetParent(transform);
        if(lockConstraints )
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
