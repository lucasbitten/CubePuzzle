using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveableFace : MonoBehaviour
{
    public enum FacePosition
    {
        Top,
        Bottom,
        Right,
        Left,
        Forward,
        Back
    }

    [SerializeField] GameEvent_Void m_onFaceMovementEndedEvent;
    [SerializeField] GameEvent_Void m_onDragEndedEvent;
    [SerializeField] Renderer m_renderer;
    [SerializeField] CubeMovementController m_cube;

    [field: SerializeField] public FacePosition CurrentFacePosition { get; private set; }
    [field: SerializeField] public FacePosition NextPosition { get; private set; }

    Transform m_moveableFacesParent;
    public Vector3 m_targetLocation;


    MaterialPropertyBlock m_selectedMaterialPropertyBlock;
    MaterialPropertyBlock m_unselectedMaterialPropertyBlock;

    private void Awake() 
    {
        m_selectedMaterialPropertyBlock = new MaterialPropertyBlock();
        m_selectedMaterialPropertyBlock.SetColor("_BaseColor", Color.red);

        m_unselectedMaterialPropertyBlock = new MaterialPropertyBlock();
        m_unselectedMaterialPropertyBlock.SetColor("_BaseColor", Color.blue);

        m_moveableFacesParent = transform.parent;
        name = $"{CurrentFacePosition} Face";
    }

    private void OnEnable()
    {
        m_onDragEndedEvent.EventListeners += OnDragEnded;
    }

    private void OnDisable()
    {
        m_onDragEndedEvent.EventListeners -= OnDragEnded;
    }


    private void OnDragEnded(Void obj)
    {
        Unselected();
    }

    public void StartMovingFaces(Transform rotateAxis, Vector3 axis, float distanceToMove, float angle)
    {
        StartCoroutine(MoveFaces(rotateAxis, axis, distanceToMove, angle));
    }

    IEnumerator MoveFaces(Transform rotateAxis, Vector3 axis, float distanceToMove, float angle)
    {
        bool goingBack = false;
        CalculateEndPosition(distanceToMove, goingBack, angle); // Calculate separated position
        Debug.Log($"Separated Position {m_targetLocation}");
        yield return StartCoroutine(SeparateOrGatherFaces()); // Separating faces from main cube
        yield return StartCoroutine(m_cube.RotateEachFace(rotateAxis, axis, angle, Mathf.Abs(angle / 90))); // Moving faces to new cube sides
        CalculateEndPosition(-distanceToMove, !goingBack, angle); // Calculate end position
        Debug.Log($"Gather Position {m_targetLocation}");

        yield return StartCoroutine(AssembleFaces()); // Moving faces back to main cube
        m_onFaceMovementEndedEvent.Raise();
    }


    public void CalculateEndPosition(float distanceToMove, bool goingBack, float angle)
    {        
        Vector3 relativeLocation = new Vector3();

        //Start to Check in which side the face is
        switch (goingBack ? NextPosition : CurrentFacePosition)
        {
            case FacePosition.Top:
                relativeLocation = new Vector3(0, distanceToMove, 0);
                break;
            case FacePosition.Bottom:
                relativeLocation = new Vector3(0, -distanceToMove, 0);
                break;
            case FacePosition.Right:
                relativeLocation = new Vector3(distanceToMove, 0, 0);
                break;
            case FacePosition.Left:
                relativeLocation = new Vector3(-distanceToMove, 0, 0);
                break;
            case FacePosition.Forward:
                relativeLocation = new Vector3(0, 0, distanceToMove);
                break;
            case FacePosition.Back:
                relativeLocation = new Vector3(0, 0, -distanceToMove);
                break;
        }

        // Get the position where the face need to go
        m_targetLocation = transform.position + (angle == -90 && goingBack ? -relativeLocation : relativeLocation);
    }

    IEnumerator AssembleFaces()
    {
        yield return StartCoroutine(SeparateOrGatherFaces());

        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        //cube.SetBottomFace();

        transform.SetParent(m_moveableFacesParent);
        SetDirection();
    }
    IEnumerator SeparateOrGatherFaces()
    {

        float elapsedTime = 0;

        // Will need to perform some of this process and yield until next frames
        float closeEnough = 0.1f;
        float distance = (transform.position - m_targetLocation).magnitude;

        // GC will trigger unless we define this ahead of time
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        // Continue until we're there
        while(distance >= closeEnough)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Slerp(transform.position, m_targetLocation, elapsedTime/2);
            yield return wait;

            distance = (transform.position - m_targetLocation).magnitude;
        }

        // Complete the motion to prevent negligible sliding
        transform.position = m_targetLocation;
    }

    public void SetDirection()
    {
        Vector3Int upDirection = new Vector3Int(Mathf.RoundToInt(transform.up.x), Mathf.RoundToInt(transform.up.y), Mathf.RoundToInt(transform.up.z));

        if (V3Equal(upDirection, Vector3.up))
        {
            CurrentFacePosition = FacePosition.Bottom;
        }
        else if (V3Equal(upDirection, Vector3.down))
        {
            CurrentFacePosition = FacePosition.Top;
        }
        else if (V3Equal(upDirection, Vector3.right))
        {
            CurrentFacePosition = FacePosition.Left;
        }
        else if (V3Equal(upDirection, Vector3.left))
        {
            CurrentFacePosition = FacePosition.Right;
        }
        else if (V3Equal(upDirection, Vector3.forward))
        {
            CurrentFacePosition = FacePosition.Back;
        }
        else if (V3Equal(upDirection, Vector3.back))
        {
            CurrentFacePosition = FacePosition.Forward;
        }

        name = $"{CurrentFacePosition} Face";
    }

    public void SetNextPosition(RotationType rotationType, int steps)
    {
        switch (rotationType)
        {
            case RotationType.Rotate_X:
                switch (CurrentFacePosition)
                {
                    case FacePosition.Right:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Back;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Left;
                        }
                        break;
                    case FacePosition.Left:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Forward;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Right;
                        }
                        break;
                    case FacePosition.Forward:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Right;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Back;
                        }
                        break;
                    case FacePosition.Back:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Left;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Forward;
                        }
                        break;
                }
                break;
            case RotationType.Rotate_Y:
                switch (CurrentFacePosition)
                {
                    case FacePosition.Right:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Top;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Left;
                        }
                        break;
                    case FacePosition.Left:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Bottom;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Right;
                        }
                        break;
                    case FacePosition.Top:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Left;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Bottom;
                        }
                        break;
                    case FacePosition.Bottom:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Right;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Top;
                        }
                        break;
                }
                break;
            case RotationType.Rotate_Z:
                switch (CurrentFacePosition)
                {
                    case FacePosition.Top:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Forward;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Bottom;
                        }
                        break;
                    case FacePosition.Bottom:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Back;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Top;
                        }
                        break;
                    case FacePosition.Forward:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Bottom;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Back;
                        }
                        break;
                    case FacePosition.Back:
                        if (steps == 1)
                        {
                            NextPosition = FacePosition.Top;
                        }
                        else if (steps == 2)
                        {
                            NextPosition = FacePosition.Forward;
                        }
                        break;
                }
                break;
        }
    }


    public bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }

    public void Selected()
    {
        m_renderer.SetPropertyBlock(m_selectedMaterialPropertyBlock);
    }

    public void Unselected()
    {
        m_renderer.SetPropertyBlock(m_unselectedMaterialPropertyBlock);

    }


}
