using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public enum RotationType
{
    Rotate_X,
    Rotate_Y,
    Rotate_Z
}

public class CubeMovementController : MonoBehaviour
{
    [SerializeField] private LevelManager m_levelManager;
    [SerializeField] private CubeManager m_cubeManager;


    public List<MoveableFace> faces = new List<MoveableFace>();
    [SerializeField] GameObject m_facesXRotator = default;
    [SerializeField] GameObject m_facesYRotator = default;
    [SerializeField] GameObject m_facesZRotator = default;

    List<MoveableFace> m_facesToMove = new List<MoveableFace>();

    public bool m_rotating;

    Item[] m_items;

    public Transform m_moveableFacesParent;

    [SerializeField] GameEvent_Void m_onRotationEnded;
    [SerializeField] GameEvent_Void m_onRotationStarted;

    void Awake()
    {
        m_items = FindObjectsOfType<Item>();
        m_onRotationEnded.EventListeners += OnRotationEnded;
        m_onRotationStarted.EventListeners += OnRotationStarted;
        m_levelManager.SetMainCube(this);

        m_cubeManager.SetCubeMovementController(this);
        m_cubeManager.SetFaces(faces);
        m_cubeManager.SetRotators(m_facesXRotator, m_facesYRotator, m_facesZRotator);

    }
    private void OnRotationStarted(Void args)
    {
        m_rotating = true;
        Debug.Log("Rotation starting");
    }
    private void OnRotationEnded(Void args)
    {
        m_rotating = false;
        Debug.Log("Rotation ending");
    }
    public void InvokeOnRotationEnded()
    {
        m_onRotationEnded.Raise();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.X) && !m_rotating /*&& !HasItemFalling()*/)
        {
            m_cubeManager.RotateFaces(90, RotationType.Rotate_X, m_facesToMove);
        }
        if (Input.GetKeyDown(KeyCode.Y) && !m_rotating /*&& !HasItemFalling()*/)
        {
            m_cubeManager.RotateFaces(90, RotationType.Rotate_Y, m_facesToMove);
        }
        if (Input.GetKeyDown(KeyCode.Z) && !m_rotating /*&& !HasItemFalling()*/)
        {
            m_cubeManager.RotateFaces(90, RotationType.Rotate_Z, m_facesToMove);
        }
    }

    public IEnumerator RotateEachFace(Transform rotateAxis, Vector3 axis, float angle, float duration)
    {
        //yield return new WaitWhile (() => CheckIfIsMoving());

        Quaternion from = rotateAxis.rotation;
        Quaternion to = rotateAxis.rotation;
        to *= Quaternion.Euler(axis * angle);

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            rotateAxis.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rotateAxis.rotation = to;

        for (int i = 0; i < m_facesToMove.Count; i++)
        {
            int xPos = Mathf.RoundToInt(m_facesToMove[i].transform.position.x);
            int yPos = Mathf.RoundToInt(m_facesToMove[i].transform.position.y);
            int zPos = Mathf.RoundToInt(m_facesToMove[i].transform.position.z);

            m_facesToMove[i].transform.position = new Vector3Int(xPos, yPos, zPos);
            m_facesToMove[i].transform.localScale = new Vector3(m_levelManager.m_levelSize, m_levelManager.m_levelSize, m_levelManager.m_levelSize);
            // facesToMove[i].transform.parent = transform;
        }

    }

    bool CheckIfIsMoving()
    {
        foreach (MoveableFace face in m_facesToMove)
        {
            if (!face.m_movingBack && !face.m_moving)
            {
                return false;
            }
        }
        return true;
    }
    private bool HasItemFalling() {

        for ( int i = 0; i < m_items.Length; ++i ) 
        {
            if (m_items[i].falling) 
            {
                return true;
            }
        }
    
        return false;
    }


}
