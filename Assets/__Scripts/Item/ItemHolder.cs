using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class HolderSpot
    {
        public Item Item { get; set; }
        public Transform LockedItemPosition; 
    }

    [SerializeField] Collider m_holderCollider;
    [SerializeField] List<HolderSpot> m_holderSpots = new List<HolderSpot>();
    [SerializeField] GameEvent_GameObject m_onItemPickedUpEvent;

    int m_holderCapacity;
    int m_itemsCount;

    public bool IsActive { get => m_isActive; set => m_isActive = value; }

    private bool m_isActive;

    private void Awake()
    {
        m_holderCapacity = m_holderSpots.Count;
    }

    private void OnEnable()
    {
        m_onItemPickedUpEvent.EventListeners += OnItemPickedUp;
    }

    private void OnDisable()
    {
        m_onItemPickedUpEvent.EventListeners -= OnItemPickedUp;
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

        item.transform.SetParent(transform);

        foreach (var spot in m_holderSpots)
        {
            if(spot.Item == null)
            {
                item.transform.position = spot.LockedItemPosition.position;
                item.SetState(Item.ItemState.OnItemHolder);
                spot.Item = item;
                m_itemsCount++;
                break;
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H)) 
        {
            UnlockAllItems();
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
        spot.Item.DetachItem();
        spot.Item = null;
        m_itemsCount--;
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
            LockItem(item);
        }
    }

    public void OnActivated()
    {
        m_holderCollider.enabled = true;
    }

    public void OnDeactivated()
    {
        UnlockAllItems();
        m_holderCollider.enabled = false;
    }
}
