using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{

    Rigidbody myRigidbody;
    public bool falling;
    public bool beingCarried;

    public bool rotating;

    [SerializeField] GameEvent_Void OnRotationEnded;
    [SerializeField] GameEvent_Void OnRotationStarted;

    public LayerMask raycastLayer;
    
    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(9,10);

        OnRotationStarted.EventListeners += Cube_OnRotationStarted;
        OnRotationEnded.EventListeners += Cube_OnRotationEnded;
    }
    private void OnDestroy()
    {
        OnRotationStarted.EventListeners -= Cube_OnRotationStarted;
        OnRotationEnded.EventListeners -= Cube_OnRotationEnded;
    }
    private void Cube_OnRotationEnded(Void args)
    {
        UnlockMovement();
    }
    private void Cube_OnRotationStarted(Void args)
    {
        LockMovement();
    }

    public void LockMovement()
    {
        myRigidbody.constraints = RigidbodyConstraints.FreezeAll;   
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
        Physics.Raycast(ray, out RaycastHit hit, 0.5f, raycastLayer);
        
        if(hit.collider)
        {
            if (hit.collider.CompareTag("Obstacles") || hit.collider.CompareTag("Faces"))
            {
                falling = false;
                Debug.LogWarning("Not Falling");
            }
            else if (hit.collider.CompareTag("Items"))
            {
                //Check if item is above other item
                Physics.Raycast(hit.collider.transform.position, Vector3.down, out RaycastHit itemHit, 0.5f, raycastLayer);
                if (itemHit.collider && (itemHit.collider.CompareTag("Obstacles") || itemHit.collider.CompareTag("Faces")))
                {
                    falling = false;
                    Debug.LogWarning("Not Falling too");

                }
                else
                {
                    falling = true;
                    Debug.LogWarning("Faaaalling");

                }
            }
        }
        else
        {
            falling = true;
            Debug.LogWarning("Falling");

        }

        rotating = false;        
    }

    private void OnCollisionEnter(Collision other)
    {
        if(falling)
        {
            if(other.gameObject.CompareTag("Items"))
            {   
                if (other.transform.parent)
                {
                    transform.parent = other.transform.parent.transform;
                } 
                else
                {
                    //transform.parent = cube.transform;
                }
                rotating = false;
                falling = false;
            } 

            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

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
                    transform.SetParent(other.transform.parent);
                }
            }
        }

        if(beingCarried)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                // pickupItem.Drop();
            }
        }

        if(other.gameObject.CompareTag("Faces"))
        {
            var face = other.gameObject.GetComponent<MoveableFace>();

            if ((face && face.CurrentFacePosition == MoveableFace.FacePosition.Bottom) || other.gameObject.CompareTag("Obstacles"))
            {
                transform.SetParent(other.transform);
                falling = false;
                rotating = false;
            }
        }
    }

    public void ClearConstraints()
    {
        myRigidbody.constraints = RigidbodyConstraints.None;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Goal")
        {
            Debug.Log("Level Completo");
        }
    }


}
