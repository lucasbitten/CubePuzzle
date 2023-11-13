using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/Cube Manager")]
public class CubeManager : ScriptableObject
{
    [SerializeField] GameEvent_Void m_onRotationEnded;
    [SerializeField] GameEvent_Void m_onRotationStarted;
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

    public void RotateFaces(float angle, RotationType rotationType, List<MoveableFace> facesToMove)
    {
        m_onRotationStarted.Raise();
        facesToMove.Clear();

        Transform facesRotationParent = null;
        List<MoveableFace.FacePosition> positionsToMove = System.Enum.GetValues(typeof(MoveableFace.FacePosition)).Cast<MoveableFace.FacePosition>().ToList();
        Vector3 axis = Vector3.zero;
        switch (rotationType)
        {
            case RotationType.Rotate_X:
                facesRotationParent = m_facesXRotation.transform;
                positionsToMove.Remove(MoveableFace.FacePosition.Up);
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


        for (int i = 0; i < m_faces.Count; i++)
        {
            if (positionsToMove.Contains(m_faces[i].m_facePosition))
            {
                m_faces[i].transform.SetParent(facesRotationParent);
                m_faces[i].SetNextPosition(rotationType);
                m_faces[i].StartMovingFaces(facesRotationParent, axis + m_cubeMovementController.transform.rotation.eulerAngles, DistanceToMove, angle);
                facesToMove.Add(m_faces[i]);
            }
        }
    }


}
