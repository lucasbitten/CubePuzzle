using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.GraphicsBuffer;

public class Item : MonoBehaviour, IAttachable
{
    public enum ItemState
    {
        Invalid = -1,
        Falling,
        BeingCarried,
        OnObstacle,
        OnItem,
        OnBottom,
        ItemLocked,
        OnButton
    }


    //todo: fix item not attaching to correct face

    [SerializeField, Required] CubeManager m_cubeManager;
    [SerializeField, Required] Rigidbody m_myRigidbody;
    [SerializeField, Required] ItemsParentValue m_itemsParentValue;

    [SerializeField, Required] GameEvent_Void m_onPreRotationStartedEvent;
    [SerializeField, Required] GameEvent_Void m_onRotationStartedEvent;
    [SerializeField, Required] GameEvent_Void m_onRotationEndedEvent;
    [SerializeField, Required] GameEvent_GameObject m_onInteractableChanged;

    [SerializeField] LayerMask m_objectsToAttachLayer;

    [SerializeField] Transform m_objectToAttach;
    [field: SerializeField] public ItemState ItemCurrentState { get; private set; } = ItemState.Invalid;
    [SerializeField] float m_raycastDistance = 0.75f;

    WaitForSeconds m_waitForSeconds;
    bool m_lockMovement;


    private void Awake()
    {
        m_waitForSeconds = new WaitForSeconds(0.1f);
    }
    private void Start()
    {
        Physics.IgnoreLayerCollision(9,10);
        CheckUnderneath();
    }

    private void OnEnable()
    {
        m_onPreRotationStartedEvent.EventListeners += OnPreRotationStarted;
        m_onRotationStartedEvent.EventListeners += OnRotationStarted;
        m_onRotationEndedEvent.EventListeners += OnRotationEnded;
        m_onInteractableChanged.EventListeners += OnInteractableChanged;
    }

    private void OnDisable()
    {
        m_onPreRotationStartedEvent.EventListeners -= OnPreRotationStarted;
        m_onRotationStartedEvent.EventListeners -= OnRotationStarted;
        m_onRotationEndedEvent.EventListeners -= OnRotationEnded;
        m_onInteractableChanged.EventListeners -= OnInteractableChanged;
    }

    private void OnRotationEnded(Void args)
    {
        UnlockMovement();
    }
    private void OnRotationStarted(Void args)
    {
        LockMovement();
    }


    private void OnInteractableChanged(GameObject args)
    {
        if(ItemCurrentState != ItemState.BeingCarried)
        {
            StartCoroutine(WaitToCheckItem());
        }
    }

    IEnumerator WaitToCheckItem()
    {
        yield return m_waitForSeconds;
        UnlockMovement();
    }

    private void OnPreRotationStarted(Void obj)
    {
        //attach item to parent
        if (m_objectToAttach != null)
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

    public ItemState OnAttach()
    {
        return ItemState.OnItem;
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
        var ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * m_raycastDistance, Color.yellow);
    }

    public void UnlockMovement()
    {
        if(ItemCurrentState == ItemState.ItemLocked)
        {
            return;
        }

        transform.SetParent(m_itemsParentValue.Value.transform);

        CheckUnderneath();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            OnPreRotationStarted(new Void());
        }


        if (ItemCurrentState == ItemState.ItemLocked)
        {
            return;
        }

        CheckWhereItemHasFallen();
    }

    public bool CheckCollision()
    {
        if(m_myRigidbody.velocity.sqrMagnitude > 0)
        {
            return false;
        }


        var ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, m_raycastDistance, m_objectsToAttachLayer))
        {
            var hitGameObject = hitInfo.collider.gameObject;

            if(hitGameObject != null && hitGameObject.TryGetComponent(out IAttachable attachable))
            {
                var state = attachable.OnAttach();
                AttachToObject(hitGameObject.transform, state);
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
        transform.SetParent(m_itemsParentValue.Value.transform);
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
