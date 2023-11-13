using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/Level Manager")]
public class LevelManager : ScriptableObject
{
    public float m_levelSize;

    Switches[] m_levelSwitches;
    CubeMovementController m_mainCube;
    ForceFieldCube m_forceField;

    List<GameObject> m_faces = new List<GameObject>();
    List<GameObject> m_forceFieldFaces = new List<GameObject>();

    public float m_position;

    public void SetMainCube(CubeMovementController cube)
    {
        m_mainCube = cube;

        for (int i = 0; i < m_mainCube.faces.Count; i++)
        {
            m_faces.Add(m_mainCube.faces[i].gameObject);
            m_forceFieldFaces.Add(m_forceField.faces[i]);
        }

        m_position = m_levelSize * 5;

        m_levelSwitches = FindObjectsOfType<Switches>();

    }
    public void SetForceFieldCube(ForceFieldCube forceFieldCube)
    {
        m_forceField = forceFieldCube;
    }




    public void CheckSwitches(){

        for (int i = 0; i < m_levelSwitches.Length; i++)
        {
            if (!m_levelSwitches[i].pressed)
            {
                return;
            }
        }

        Debug.Log("Level Completo!");

    }

    public void AdjustLevel()
    {
        for (int i = 0; i < m_faces.Count; i++)
        {
            Vector3 pos = m_faces[i].transform.localPosition;

            m_faces[i].transform.localScale = Vector3.one * m_levelSize;
            m_forceFieldFaces[i].transform.localScale = new Vector3 (m_levelSize*10, 0.05f, m_levelSize*10);

            if (Mathf.Abs(pos.x) > Mathf.Abs(pos.y) && Mathf.Abs(pos.x) > Mathf.Abs(pos.z))
            {                    
                if (pos.x > 0)
                {
                    m_faces[i].transform.localPosition = Vector3.right * m_levelSize * 5;
                    m_forceFieldFaces[i].transform.localPosition = Vector3.right * m_levelSize * 5 - new Vector3 (0.02f,0,0);
                }
                else
                {
                    m_faces[i].transform.localPosition = Vector3.left * m_levelSize * 5;
                    m_forceFieldFaces[i].transform.localPosition = Vector3.left * m_levelSize * 5 + new Vector3 (0.02f,0,0);
                }
            } 
            else if (Mathf.Abs(pos.y) > Mathf.Abs(pos.z))
            {

                if (pos.y > 0)
                {
                    m_faces[i].transform.localPosition = Vector3.up * m_levelSize * 5;
                    m_forceFieldFaces[i].transform.localPosition = Vector3.up * m_levelSize * 5 - new Vector3 (0,0.02f,0);

                } 
                else
                {
                    m_faces[i].transform.localPosition = Vector3.down * m_levelSize * 5;
                    m_forceFieldFaces[i].transform.localPosition = Vector3.down * m_levelSize * 5 + new Vector3 (0,0.02f,0);

                }
            }
            else
            {
                if (pos.z > 0)
                {
                    m_faces[i].transform.localPosition = Vector3.forward * m_levelSize * 5;
                    m_forceFieldFaces[i].transform.localPosition = Vector3.forward * m_levelSize * 5 - new Vector3 (0,0,0.02f);


                } else
                {
                    m_faces[i].transform.localPosition = Vector3.back * m_levelSize * 5;
                    m_forceFieldFaces[i].transform.localPosition = Vector3.back * m_levelSize * 5 + new Vector3 (0,0,0.02f);
                }
            }
        }

        m_position = m_levelSize * 5;
        Debug.Log("Ajustou o level");
    }




}
