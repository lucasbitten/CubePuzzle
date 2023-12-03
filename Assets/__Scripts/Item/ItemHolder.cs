using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour, IInteractable, IAttachable
{
    [System.Serializable]
    public class HolderSpot
    {
        public Item Item { get; set; }
        public Transform LockedItemPosition; 
    }

    [SerializeField, Required] Collider m_holderCollider;
    [SerializeField, Required] GameEvent_GameObject m_onItemPickedUpEvent;
    [SerializeField, Required] GameObject m_activeVisual;

    [SerializeField] bool m_isActive;
    [SerializeField] List<HolderSpot> m_holderSpots = new List<HolderSpot>();

    int m_holderCapacity;
    int m_itemsCount;

    public bool IsActive { get => m_isActive; set => m_isActive = value; }


    private void Awake()
    {
        m_holderCapacity = m_holderSpots.Count;
        if(m_activeVisual)
        {
            m_activeVisual.SetActive(IsActive);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            UnlockAllItems();
        }
    }


    private void OnEnable()
    {
        m_onItemPickedUpEvent.EventListeners += OnItemPickedUp;
    }

    private void OnDisable()
    {
        m_onItemPickedUpEvent.EventListeners -= OnItemPickedUp;
    }

    public Item.ItemState OnAttach()
    {
        return Item.ItemState.ItemLocked;
    }

    private void OnItemPickedUp(GameObject item)
    {   
        if(m_holderSpots.Count > 0)
        {
            foreach(var spot in m_holderSpots)
            {
                if(spot.Item != null && spot.Item.gameObject == item)
                {
                    spot.Item = null;
                    m_itemsCount--;
                    break;
                }
            }

        }
    }
    void LockItem(Item item)
    {
        if (m_itemsCount == m_holderCapacity)
        {
            Debug.Log($"Holder {gameObject.name} is already full", gameObject);
            return;
        }


        foreach (var spot in m_holderSpots)
        {
            if(spot.Item == null)
            {
                item.transform.SetParent(spot.LockedItemPosition);
                item.transform.position = spot.LockedItemPosition.position;
                item.transform.rotation = Quaternion.identity;
                item.SetState(Item.ItemState.ItemLocked);
                LockItemRigidbody(item, true);
                spot.Item = item;
                m_itemsCount++;
                break;
            }
        }
    }

    void UnlockAllItems()
    {
        foreach (var spot in m_holderSpots)
        {
            if(spot.Item != null)
            {
                UnlockItem(spot);
            }
        }
    }

    void UnlockItem(HolderSpot spot)
    {
        LockItemRigidbody(spot.Item,false);
        spot.Item.DetachItem();
        spot.Item = null;
        m_itemsCount--;
    }

    void LockItemRigidbody(Item item, bool isLocked)
    {
        if(item.TryGetComponent(out Rigidbody rigidyBody))
        {
            rigidyBody.useGravity = !isLocked;
            rigidyBody.constraints = isLocked ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;            
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (m_itemsCount == m_holderCapacity)
        {
            Debug.Log($"Holder {gameObject.name} is already full", gameObject);
            return;
        }

        if (other.gameObject.TryGetComponent(out Item item))
        {
            if(item.ItemCurrentState != Item.ItemState.BeingCarried)
            {
                LockItem(item);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_itemsCount == m_holderCapacity)
        {
            return;
        }

        if (other.gameObject.TryGetComponent(out Item item))
        {
            foreach (var spots in m_holderSpots)
            {
                if(item ==  spots.Item)
                {
                    return;
                }
            }


            if (item.ItemCurrentState != Item.ItemState.BeingCarried)
            {
                LockItem(item);
            }
        }
    }




    public void OnActivated()
    {
        m_isActive = true;
        m_holderCollider.enabled = true;
        if(m_activeVisual != null)
        {
            m_activeVisual.SetActive(m_isActive);
        }

    }

    public void OnDeactivated()
    {
        m_isActive = false;
        UnlockAllItems();
        m_holderCollider.enabled = false;
        if(m_activeVisual != null)
        {
            m_activeVisual.SetActive(m_isActive);
        }

    }
}
