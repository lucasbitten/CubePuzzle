using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switches : MonoBehaviour
{
    [SerializeField, Required] LevelManager m_levelManager;
    public bool pressed;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Items") || other.CompareTag("Player"))
        { 
            pressed = true;
            m_levelManager.CheckSwitches();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Items") || other.CompareTag("Player"))
        { 
            pressed = false;
            m_levelManager.CheckSwitches();
        }
    }

}
