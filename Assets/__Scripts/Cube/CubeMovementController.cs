using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum RotationType
{
    Rotate_X,
    Rotate_Y,
    Rotate_Z
}

public class CubeMovementController : MonoBehaviour
{
    public class RotationInfo
    {
        public RotationInfo(float angle, RotationType rotationType, List<MoveableFace> faces)
        {
            Angle = angle;
            RotationType = rotationType;
            FacesToMove = faces;
        }

        public float Angle { get; private set; }
        public RotationType RotationType { get; private set; }
        public List<MoveableFace> FacesToMove { get; private set; }
    }


    [SerializeField] private LevelManager m_levelManager;
    [SerializeField] private CubeManager m_cubeManager;

    [SerializeField] GameObject m_facesXRotator = default;
    [SerializeField] GameObject m_facesYRotator = default;
    [SerializeField] GameObject m_facesZRotator = default;

    [SerializeField] GameEvent_Void m_onRotationStartedEvent;
    [SerializeField] GameEvent_Void m_onRotationEndedEvent;
    [SerializeField] GameEvent_Int m_onFacesStartedRotatingEvent;
    [SerializeField] GameEvent_Void m_onFaceMovementEndedEvent;
    [SerializeField] GameEvent_Void m_onDragEndedEvent;

    [field: SerializeField] public List<MoveableFace> Faces { get; private set; } = new List<MoveableFace>();

    List<MoveableFace> m_facesToMove = new List<MoveableFace>();

    public bool IsRotating { get; private set; }
    List<Item> m_items;
    private int m_facesRotating;
    RotationInfo m_currentRotationInfo;

    [SerializeField] float m_levelSize;

    void Awake()
    {
        m_items = GetComponentsInChildren<Item>().ToList();

        m_levelManager.SetMainCube(this);

        m_cubeManager.SetCubeMovementController(this);
        m_cubeManager.SetFaces(Faces);
        m_cubeManager.SetRotators(m_facesXRotator, m_facesYRotator, m_facesZRotator);

    }

    private void OnEnable()
    {
        m_onRotationEndedEvent.EventListeners += OnRotationEnded;
        m_onRotationStartedEvent.EventListeners += OnRotationStarted;
        m_onFacesStartedRotatingEvent.EventListeners += OnFacesStartedRotating;
        m_onFaceMovementEndedEvent.EventListeners += OnFaceMovementEnded;
        m_onDragEndedEvent.EventListeners += OnDragEnded;
    }
    private void OnDisable()
    {
        m_onRotationEndedEvent.EventListeners -= OnRotationEnded;
        m_onRotationStartedEvent.EventListeners -= OnRotationStarted;
        m_onFacesStartedRotatingEvent.EventListeners -= OnFacesStartedRotating;
        m_onFaceMovementEndedEvent.EventListeners -= OnFaceMovementEnded;
        m_onDragEndedEvent.EventListeners -= OnDragEnded;
    }

    private void OnFaceMovementEnded(Void arg)
    {
        m_facesRotating--;

        if(m_facesRotating == 0)
        {
            m_onRotationEndedEvent.Raise();
        }

    }

    private void OnFacesStartedRotating(int args)
    {
        m_facesRotating = args;
    }

    private void OnRotationStarted(Void args)
    {
        IsRotating = true;
        Debug.Log("Rotation starting");
    }
    private void OnRotationEnded(Void args)
    {
        IsRotating = false;

        m_facesXRotator.transform.rotation = Quaternion.identity;
        m_facesYRotator.transform.rotation = Quaternion.identity;
        m_facesZRotator.transform.rotation = Quaternion.identity;

        Debug.Log("Rotation ending");
    }
    public void InvokeOnRotationEnded()
    {
        m_onRotationEndedEvent.Raise();
    }
    private void OnDragEnded(Void args)
    {
        if (m_currentRotationInfo != null)
        {
            RotateCube(m_currentRotationInfo);
        }

        m_currentRotationInfo = null;
    }


    public void SetLevelSize(float levelSize)
    {
        m_levelSize = levelSize;
    }

    public void SetRotationInfo(RotationInfo rotationInfo)
    {
        m_currentRotationInfo = rotationInfo;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.X) && !IsRotating /*&& !HasItemFalling()*/)
        {
            RotateCube(new RotationInfo(90, RotationType.Rotate_X, m_facesToMove));
        }
        if (Input.GetKeyDown(KeyCode.Y) && !IsRotating /*&& !HasItemFalling()*/)
        {
            RotateCube(new RotationInfo(90, RotationType.Rotate_Y, m_facesToMove));
        }
        if (Input.GetKeyDown(KeyCode.Z) && !IsRotating /*&& !HasItemFalling()*/)
        {
            RotateCube(new RotationInfo(90, RotationType.Rotate_Z, m_facesToMove));
        }
    }
    public void RotateCube(RotationInfo rotationInfo)
    {
        m_cubeManager.RotateCube(rotationInfo);
    }
    public IEnumerator RotateEachFace(Transform rotateAxis, Vector3 axis, float angle, float duration)
    {
        //yield return new WaitWhile (() => CheckIfIsMoving());

        var start = rotateAxis.rotation;
        float t = 0f;

        while (t < duration)
        {
            rotateAxis.rotation = start * Quaternion.AngleAxis(angle * t / duration, axis);
            yield return null;
            t += Time.deltaTime;
        }

        rotateAxis.rotation = start * Quaternion.AngleAxis(angle, axis);

        for (int i = 0; i < m_facesToMove.Count; i++)
        {
            int xPos = Mathf.RoundToInt(m_facesToMove[i].transform.position.x);
            int yPos = Mathf.RoundToInt(m_facesToMove[i].transform.position.y);
            int zPos = Mathf.RoundToInt(m_facesToMove[i].transform.position.z);

            m_facesToMove[i].transform.position = new Vector3Int(xPos, yPos, zPos);
            m_facesToMove[i].transform.localScale = new Vector3(m_levelSize, m_levelSize, m_levelSize);
        }

    }
    public bool HasItemFalling() {

        for ( int i = 0; i < m_items.Count; ++i ) 
        {
            if (m_items[i].ItemCurrentState == Item.ItemState.Falling) 
            {
                return true;
            }
        }
    
        return false;
    }
}
