using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WebGLPlayerController : MonoBehaviour
{
    [SerializeField]
    Camera webGLCamera;

    [SerializeField]
    Tool activeTool;

    [SerializeField]
    InputActionAsset inputActions;

    [SerializeField]
    private List<GameObject> tools;

    private InputAction mousePositionAction;

    private InputAction switchToolAction;

    private Vector3 playerPosition;

    private IGrabbable grabbable;

    private GameObject activeToolObject;


    private void OnEnable()
    {
        mousePositionAction = inputActions.FindAction("Position");
        switchToolAction = inputActions.FindAction("SwitchTools");

        if (mousePositionAction != null)
        {
            mousePositionAction.performed += inputContext => UpdatePlayerPosition(inputContext);
        }

        if (switchToolAction != null)
        {
            switchToolAction.performed += inputContext => SwitchTools(inputContext);
        }

    }

    private void Start()
    {
        playerPosition = Vector3.zero;
    }

    private void UpdatePlayerPosition(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input != null)
        {
            Vector3 mouseViewportPos = webGLCamera.ScreenToViewportPoint(input);
            mouseViewportPos.z = 1.6f;
            playerPosition = webGLCamera.ViewportToWorldPoint(mouseViewportPos);
            transform.position = playerPosition;
        }
    }

    private void SwitchTools(InputAction.CallbackContext context)
    {
        //NOTE: Currently the only possible tool is Scissors, so I just hardcode that for now - must change later.
        if (activeTool == null)
        {
            activeToolObject = tools[0];
            activeToolObject.transform.SetParent(transform);
            activeToolObject.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            activeTool = tools[0].GetComponent<Tool>();
            activeTool.WebGLMakeActiveTool();
        }
        else
        {
            activeToolObject.transform.SetParent(null);
            activeTool.WebGLSwitchToDifferentTool();
            activeTool = null;
        }
    }

    private void OnDisable()
    {
        if (mousePositionAction != null)
        {
            mousePositionAction.performed -= inputContext => UpdatePlayerPosition(inputContext);
        }

        if (switchToolAction != null)
        {
            switchToolAction.performed -= inputContext => SwitchTools(inputContext);
        }

    }
}