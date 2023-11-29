using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public interface IAttachable
{
    public ItemState OnAttach();
}
