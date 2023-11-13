using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGameEventListener<T, GE, UER> : MonoBehaviour
    where GE : BaseGameEvent<T>
    where UER : UnityEvent<T>
{
    [SerializeField] 
    protected GE m_gameEvent;

    [SerializeField] 
    protected UER m_unityEventResponse;

    private void OnEnable()
    {
        if(m_gameEvent == null) 
        { 
            return;
        }

        m_gameEvent.EventListeners += TriggerResponses; // Subscribe
    }

    private void TriggerResponses(T val)
    {
        //No need to nullcheck here, UnityEvents do that for us (lets avoid the double nullcheck)
        m_unityEventResponse.Invoke(val);
    }

    private void OnDisable()
    {
        if (m_gameEvent == null)
        {
            return;
        }
        
        m_gameEvent.EventListeners -= TriggerResponses; // Unsubscribe
    }

}
