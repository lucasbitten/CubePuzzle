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
    float position;
    [SerializeField]
    GameObject facesXRotation = default;
    [SerializeField]
    GameObject facesYRotation = default;
    [SerializeField]
    GameObject facesZRotation = default;


    List<MoveableFace> facesToMove = new List<MoveableFace>();

    public bool rotating;

    Item[] items;

    public Transform moveableFacesParent;

    [SerializeField] GameEvent_Void OnRotationEnded;
    [SerializeField] GameEvent_Void OnRotationStarted;

    void Awake()
    {
        items = FindObjectsOfType<Item>();
        OnRotationEnded.EventListeners += CubeMovement_OnRotationEnded;
        OnRotationStarted.EventListeners += CubeMovement_OnRotationStarted;
        m_levelManager.SetMainCube(this);

        m_cubeManager.SetCubeMovementController(this);
        m_cubeManager.SetFaces(faces);
        m_cubeManager.SetRotators(facesXRotation, facesYRotation, facesZRotation);

    }
    private void CubeMovement_OnRotationStarted(Void args)
    {
        rotating = true;
        Debug.Log("Rotation starting");
    }
    private void CubeMovement_OnRotationEnded(Void args)
    {
        rotating = false;
        Debug.Log("Rotation ending");
    }
    public void Invoke_CubeMovement_OnRotationEnded()
    {
        OnRotationEnded.Raise();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.X) && !rotating && !HasItemFalling())
        {
            m_cubeManager.RotateFaces(90, RotationType.Rotate_X, facesToMove);
        }
        if (Input.GetKeyDown(KeyCode.Y) && !rotating && !HasItemFalling())
        {
            m_cubeManager.RotateFaces(90, RotationType.Rotate_Y, facesToMove);
        }
        if (Input.GetKeyDown(KeyCode.Z) && !rotating && !HasItemFalling())
        {
            m_cubeManager.RotateFaces(90, RotationType.Rotate_Z, facesToMove);
        }


        //if (Input.GetKeyDown(KeyCode.O) && !rotating && !HasItemFalling())
        //{
        //    RotateFullCube(90, Vector3.up);
        //}
        //if (Input.GetKeyDown(KeyCode.P) && !rotating && !HasItemFalling())
        //{
        //    RotateFullCube(-90, Vector3.up);
        //}
        //if (Input.GetKeyDown(KeyCode.K) && !rotating && !HasItemFalling())
        //{
        //    RotateFullCube(90, Vector3.right);
        //}
        //if (Input.GetKeyDown(KeyCode.L) && !rotating && !HasItemFalling())
        //{
        //    RotateFullCube(-90, Vector3.right);
        //}
        //if (Input.GetKeyDown(KeyCode.N) && !rotating && !HasItemFalling())
        //{
        //    RotateFullCube(90, Vector3.forward);
        //}
        //if (Input.GetKeyDown(KeyCode.M) && !rotating && !HasItemFalling())
        //{
        //    RotateFullCube(-90, Vector3.forward);
        //}
    }

    //void RotateFullCube(float angle, Vector3 axis)
    //{
    //    OnRotationStarted.Raise();
    //    StartCoroutine(RotateFullCube(axis, angle, Mathf.Abs(angle / 90)));
    //}

    //public IEnumerator RotateFullCube(Vector3 axis, float angle, float duration)
    //{
    //    Quaternion from = moveableFacesParent.rotation;
    //    Quaternion to = moveableFacesParent.rotation;
    //    to *= Quaternion.Euler(axis * angle);

    //    float elapsed = 0.0f;
    //    while (elapsed < duration)
    //    {
    //        moveableFacesParent.rotation = Quaternion.Slerp(from, to, elapsed / duration);
    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }
    //    moveableFacesParent.rotation = to;
    //    rotating = false;

    //    foreach (var face in faces)
    //    {
    //        face.SetDirection();
    //    }
    //    OnRotationEnded.Raise();
    //}

    public IEnumerator RotateEachFace(Transform rotateAxis, Vector3 axis, float angle, float duration)
    {
        yield return new WaitWhile (() => CheckIfIsMoving());

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

        for (int i = 0; i < facesToMove.Count; i++)
        {
            int xPos = Mathf.RoundToInt(facesToMove[i].transform.position.x);
            int yPos = Mathf.RoundToInt(facesToMove[i].transform.position.y);
            int zPos = Mathf.RoundToInt(facesToMove[i].transform.position.z);

            facesToMove[i].transform.position = new Vector3Int(xPos, yPos, zPos);
            facesToMove[i].transform.localScale = new Vector3(m_levelManager.m_levelSize, m_levelManager.m_levelSize, m_levelManager.m_levelSize);
            // facesToMove[i].transform.parent = transform;
        }

        for (int i = 0; i < facesToMove.Count; i++)
        {
            facesToMove[i].CalculateMovement(-m_cubeManager.DistanceToMove, true);
        }
    }

    bool CheckIfIsMoving()
    {
        for (int i = 0; i < facesToMove.Count; i++)
        {
            if (!facesToMove[i].movingBack && !facesToMove[i].moving)
            {
                return false;
            }
        }
        return true;
    }
    private bool HasItemFalling() {

        for ( int i = 0; i < items.Length; ++i ) 
        {
            if (items[i].falling) 
            {
                return true;
            }
        }
    
        return false;
    }


}
