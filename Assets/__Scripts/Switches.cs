using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switches : MonoBehaviour
{
    [SerializeField] LevelManager m_levelManager;
    public bool pressed;
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Items"){
            pressed = true;
            m_levelManager.CheckSwitches();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Items"){
            pressed = false;
            m_levelManager.CheckSwitches();
        }
    }

}
