using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldCube : MonoBehaviour
{
    [SerializeField] LevelManager m_levelManager;

    private void Awake()
    {
        m_levelManager.SetForceFieldCube(this);
    }
    public GameObject[] faces;
}
