using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacePosition
{
    Up,
    Bottom,
    Right,
    Left,
    Forward,
    Back
}
public class MoveableFace : MonoBehaviour
{
    public bool moving;
    public bool movingBack;
    CubeMovementController cube;
    public Vector3 targetLocation;

    public FacePosition facePosition;
    public FacePosition nextPosition;
    Transform moveableFacesParent;

    private void Awake() 
    {
        cube = FindObjectOfType<CubeMovementController>();
        moveableFacesParent = transform.parent;
        name = $"{facePosition} Face";
    }
    public void Move(Transform rotateAxis, Vector3 axis, float distanceToMove, float angle)
    {
        CalculateMovement(distanceToMove, false);
        StartCoroutine(cube.RotateEachFace(rotateAxis, axis, angle, Mathf.Abs(angle / 90)));
    }
    public void CalculateMovement(float distanceToMove, bool goingBack)
    {        
        Vector3 relativeLocation = new Vector3();

        //Start to Check in which side the face is
        switch (goingBack ? nextPosition : facePosition)
        {
            case FacePosition.Up:
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
        targetLocation = transform.position + relativeLocation;

        StartCoroutine(MoveFace(targetLocation, goingBack));

    }
    public IEnumerator MoveFace(Vector3 target, bool goingBack)
    {

        float elapsedTime = 0;

        if(goingBack)
        {
            movingBack = true;
        } 
        else
        {
            moving = true;
        }

        // Will need to perform some of this process and yield until next frames
        float closeEnough = 0.1f;
        float distance = (transform.position - target).magnitude;

        // GC will trigger unless we define this ahead of time
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        // Continue until we're there
        while(distance >= closeEnough)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Slerp(transform.position, target, elapsedTime/2);
            yield return wait;

            distance = (transform.position - target).magnitude;
        }

        // Complete the motion to prevent negligible sliding
        transform.position = target;

        if (goingBack)
        {
            transform.position = new Vector3 (Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z) );
            movingBack = false;
            //cube.SetBottomFace();

            transform.SetParent(moveableFacesParent);
            SetDirection();
            cube.Invoke_CubeMovement_OnRotationEnded();
        }
        else
        {
            moving = false;
        }    
    }
    public void SetDirection()
    {
        Vector3Int upDirection = new Vector3Int(Mathf.RoundToInt(transform.up.x), Mathf.RoundToInt(transform.up.y), Mathf.RoundToInt(transform.up.z));

        if (V3Equal(upDirection, Vector3.up))
        {
            facePosition = FacePosition.Bottom;
        }
        else if (V3Equal(upDirection, Vector3.down))
        {
            facePosition = FacePosition.Up;
        }
        else if (V3Equal(upDirection, Vector3.right))
        {
            facePosition = FacePosition.Left;
        }
        else if (V3Equal(upDirection, Vector3.left))
        {
            facePosition = FacePosition.Right;
        }
        else if (V3Equal(upDirection, Vector3.forward))
        {
            facePosition = FacePosition.Back;
        }
        else if (V3Equal(upDirection, Vector3.back))
        {
            facePosition = FacePosition.Forward;
        }

        name = $"{facePosition} Face";
    }
    public void SetNextPosition(RotationType rotationType)
    {
        switch (rotationType)
        {
            case RotationType.Rotate_X:
                switch (facePosition)
                {
                    case FacePosition.Right:
                        nextPosition = FacePosition.Back;
                        break;
                    case FacePosition.Left:
                        nextPosition = FacePosition.Forward;
                        break;
                    case FacePosition.Forward:
                        nextPosition = FacePosition.Right;
                        break;
                    case FacePosition.Back:
                        nextPosition = FacePosition.Left;
                        break;
                }
                break;
            case RotationType.Rotate_Y:
                switch (facePosition)
                {
                    case FacePosition.Right:
                        nextPosition = FacePosition.Up;
                        break;
                    case FacePosition.Left:
                        nextPosition = FacePosition.Bottom;
                        break;
                    case FacePosition.Up:
                        nextPosition = FacePosition.Left;
                        break;
                    case FacePosition.Bottom:
                        nextPosition = FacePosition.Right;
                        break;
                }
                break;
            case RotationType.Rotate_Z:
                switch (facePosition)
                {
                    case FacePosition.Up:
                        nextPosition = FacePosition.Forward;
                        break;
                    case FacePosition.Bottom:
                        nextPosition = FacePosition.Back;
                        break;
                    case FacePosition.Forward:
                        nextPosition = FacePosition.Bottom;
                        break;
                    case FacePosition.Back:
                        nextPosition = FacePosition.Up;
                        break;
                }
                break;
        }
    }
    public bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }

}
