using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignXRRotation : MonoBehaviour
{
    [SerializeField] private Transform xrCamera;
    [SerializeField] private Transform desiredForwardReference;

    private void Start()
    {
        Vector3 headsetForward = Camera.main.transform.forward;
        headsetForward.y = 0f;

        Vector3 desiredForward = desiredForwardReference.forward;
        desiredForward.y = 0f;

        float angleDifference = Vector3.SignedAngle(headsetForward, desiredForward, Vector3.up);

        xrCamera.Rotate(0, angleDifference, 0, Space.World);
    }
}
