using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WebGLCameraController : MonoBehaviour
{
    [SerializeField]
    GameObject tree;

    InputAction rotateCamera;

    private void OnEnable()
    {
        rotateCamera = PlayerInputManager.Instance.inputActions.FindAction("RotateView");

        if (rotateCamera != null)
        {
            rotateCamera.performed += inputContext => OnRotateView(inputContext);
        }
    }

    private void OnRotateView(InputAction.CallbackContext context)
    {
/*        Debug.Log(context);
        Vector2 input = context.ReadValue<Vector2>();
        Vector2 newRotation = new Vector3(transform.rotation.x - input.y, transform.rotation.y + input.x);
        transform.RotateAround(tree.transform.position, newRotation, 1);
        transform.LookAt(tree.transform.position);*/
        
    }

    private void OnDisable()
    {
        if (rotateCamera != null)
        {
            rotateCamera.performed -= inputContext => OnRotateView(inputContext);
        }
    }
}
