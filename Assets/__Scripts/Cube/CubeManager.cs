using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CubeMovementController;

[CreateAssetMenu(menuName = "Managers/Cube Manager")]
public class CubeManager : ScriptableObject
{
    [SerializeField] GameEvent_Void m_onRotationEnded;
    [SerializeField] GameEvent_Void m_onRotationStarted;
    [SerializeField] GameEvent_Void m_onDragEnded;
    [SerializeField] GameEvent_Int m_onFacesStartedRotating;

    [SerializeField] List<MoveableFace> m_faces = new List<MoveableFace>();
    [field: SerializeField] public float DistanceToMove { get; private set; } = 10;

    CubeMovementController m_cubeMovementController;

    GameObject m_facesXRotation = default;
    GameObject m_facesYRotation = default;
    GameObject m_facesZRotation = default;



    public void SetCubeMovementController(CubeMovementController cubeMovementController)
    {
        m_cubeMovementController = cubeMovementController;
    }

    public void SetRotators(GameObject facesXRotation, GameObject facesYRotation, GameObject facesZRotation)
    {
        m_facesXRotation = facesXRotation;
        m_facesYRotation = facesYRotation;
        m_facesZRotation = facesZRotation;
    }

    public void SetFaces(List<MoveableFace> faces)
    {
        m_faces = faces;
    }

    public void RotateCube(RotationInfo rotationInfo)
    {
        m_onRotationStarted.Raise();
        Transform facesRotationParent;
        List<MoveableFace.FacePosition> facesPositionsToMove;
        Vector3 axis;
        SelectFacesToRotate(rotationInfo.RotationType, rotationInfo.FacesToMove, out facesRotationParent, out facesPositionsToMove, out axis);

        m_onFacesStartedRotating.Raise(facesPositionsToMove.Count);

        foreach (MoveableFace face in m_faces)
        {
            if (facesPositionsToMove.Contains(face.CurrentFacePosition))
            {
                face.transform.SetParent(facesRotationParent);

                int steps = Mathf.RoundToInt(Mathf.Abs(rotationInfo.Angle / 90f));
                if(steps > 2)
                {
                    steps = 2;
                }

                face.SetNextPosition(rotationInfo.RotationType, steps);
                face.StartMovingFaces(facesRotationParent, axis + m_cubeMovementController.transform.rotation.eulerAngles, DistanceToMove, rotationInfo.Angle);
                rotationInfo.FacesToMove.Add(face);
            }
        }
    }

    private void SelectFacesToRotate(RotationType rotationType, List<MoveableFace> facesToMove, out Transform facesRotationParent, out List<MoveableFace.FacePosition> positionsToMove, out Vector3 axis)
    {
        facesToMove.Clear();

        facesRotationParent = null;
        positionsToMove = System.Enum.GetValues(typeof(MoveableFace.FacePosition)).Cast<MoveableFace.FacePosition>().ToList();
        axis = Vector3.zero;
        switch (rotationType)
        {
            case RotationType.Rotate_X:
                facesRotationParent = m_facesXRotation.transform;
                positionsToMove.Remove(MoveableFace.FacePosition.Top);
                positionsToMove.Remove(MoveableFace.FacePosition.Bottom);
                axis = Vector3.up;
                break;
            case RotationType.Rotate_Y:
                facesRotationParent = m_facesYRotation.transform;
                positionsToMove.Remove(MoveableFace.FacePosition.Forward);
                positionsToMove.Remove(MoveableFace.FacePosition.Back);
                axis = Vector3.forward;
                break;
            case RotationType.Rotate_Z:
                facesRotationParent = m_facesZRotation.transform;
                positionsToMove.Remove(MoveableFace.FacePosition.Right);
                positionsToMove.Remove(MoveableFace.FacePosition.Left);
                axis = Vector3.right;
                break;
        }
    }

    public void SetRotationInfo(RotationInfo rotationInfo)
    {
        if(m_cubeMovementController != null)
        {
            m_cubeMovementController.SetRotationInfo(rotationInfo);
        }
    }

    public bool IsRotating()
    {
        return m_cubeMovementController.IsRotating;
    }

    public bool HasItemFalling()
    {
        return m_cubeMovementController.HasItemFalling();
    }

    public bool CanRotate()
    {
        return !IsRotating() && !HasItemFalling();
    }

}
