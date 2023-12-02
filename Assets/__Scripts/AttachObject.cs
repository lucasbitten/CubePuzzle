using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachObject : MonoBehaviour
{

    [SerializeField] Transform m_objectToAttach;

    private Vector3 m_initialPosOffset;
    public void AttachToObject(Transform objectToAttach)
    {
        m_objectToAttach = objectToAttach;
        m_initialPosOffset = transform.position - m_objectToAttach.position; //where were we relative to it?
    }

    public void Release()
    { 
        m_objectToAttach = null; 
    }


    void Update()
    {
        if (!m_objectToAttach) return;//if we haven't hit anything yet, skip the next part
        transform.position = m_objectToAttach.position + m_initialPosOffset; //move to where the stuck object is, plus the offset
    }
}
