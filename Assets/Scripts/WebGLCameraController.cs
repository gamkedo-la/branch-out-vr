using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WebGLCameraController : MonoBehaviour
{
    [SerializeField]
    GameObject tree;

    [SerializeField]
    float rotationSpeed = 100f;

    [SerializeField]
    float minVerticalAngle = 10f;

    [SerializeField]
    float maxVerticalAngle = 80f;

    [SerializeField]
    float zoomSpeed = 2f;

    [SerializeField]
    float minDistanceFromTree = 1.0f;

    [SerializeField]
    float maxDistanceFromTree = 3.0f;

    [SerializeField] float rotationSmoothing = 5f;

    public static event Action OnCameraViewRotated;

    private Vector3 offset;

    private float currentVerticalAngle = 45f; 
    private Vector3 smoothedOffset;
    private InputAction rotateCamera;
    private InputAction zoomCamera;

    private void OnEnable()
    {
        if (PlayerInputManager.Instance != null)
        {
            rotateCamera = PlayerInputManager.Instance.inputActions.FindAction("RotateView");
            zoomCamera = PlayerInputManager.Instance.inputActions.FindAction("Zoom");
        }

        if (rotateCamera != null)
        {
            rotateCamera.performed += OnRotateView;
        }

        if (zoomCamera != null)
        {
            zoomCamera.performed += OnZoom;
        }
    }
    private void Start()
    {
        offset = transform.position - tree.transform.position;
        smoothedOffset = offset;
    }

    //NOTE/TODO: May want to implement zoom functionality
    private void OnRotateView(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        float horizontalRotation = input.x * rotationSpeed * Time.deltaTime;
        float verticalRotation = input.y * rotationSpeed * Time.deltaTime;

        //Get horizontal rotation around the tree and apply it to the offset
        Quaternion horizontal = Quaternion.AngleAxis(horizontalRotation, Vector3.up);
        offset = horizontal * offset;

        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle + verticalRotation, minVerticalAngle, maxVerticalAngle);

        //After applying horizontal rotation, get the new Right direction axis
        Vector3 rightAxis = Vector3.Cross(Vector3.up, offset).normalized;
        Quaternion vertical = Quaternion.AngleAxis(verticalRotation, rightAxis);

        //Apply vertical rotation with clamped vertical angle
        Vector3 newOffset = vertical * offset;

        //Prevent flipping by only using newOffset if it doesn't cause the angle to go below min or above max; otherwise just apply horizontal offset
        float newVerticalAngle = Vector3.Angle(Vector3.up, newOffset);
        if (newVerticalAngle > minVerticalAngle && newVerticalAngle < maxVerticalAngle)
        {
            offset = newOffset;
        }
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();

        Vector3 zoomDirection = offset.normalized;

        float distance = offset.magnitude - input * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistanceFromTree, maxDistanceFromTree);

        offset = zoomDirection * distance;
    }
    private void LateUpdate()
    {
        smoothedOffset = Vector3.Lerp(smoothedOffset, offset, rotationSmoothing * Time.deltaTime);
        transform.position = tree.transform.position + smoothedOffset;
        transform.LookAt(tree.transform.position);
        OnCameraViewRotated?.Invoke();
    }

    private void OnDisable()
    {
        if (rotateCamera != null)
        {
            rotateCamera.performed -= OnRotateView;
        }

        if (zoomCamera != null)
        {
            zoomCamera.performed -= OnZoom;
        }
    }
}
