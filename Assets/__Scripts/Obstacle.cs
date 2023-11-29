using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IAttachable
{
    public Item.ItemState OnAttach()
    {
        return Item.ItemState.OnObstacle;
    }
}
