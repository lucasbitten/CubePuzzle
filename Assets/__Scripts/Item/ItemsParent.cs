using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsParent : MonoBehaviour
{
    [SerializeField] ItemsParentValue m_itemsParentValue;

    public List<Item> Items { get; private set; }


    private void Awake()
    {
        m_itemsParentValue.SetValue(this);
    }

    private void Start()
    {
        Items = GetComponentsInChildren<Item>().ToList();
    }

}
