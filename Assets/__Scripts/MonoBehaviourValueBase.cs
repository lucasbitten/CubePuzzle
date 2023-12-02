using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoBehaviourValueBase<T> : ScriptableObject
{
    public T Value { get; private set; }


    public void SetValue(T value)
    { 
        Value = value;
    }
}
