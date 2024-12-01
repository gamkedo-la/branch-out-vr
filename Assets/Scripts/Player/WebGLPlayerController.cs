using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

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

    [SerializeField]
    GameObject hand;

    private HandPoseController handPoseController;

    private InputAction mousePositionAction;

    private InputAction switchToolAction;

    private InputAction useToolAction;

    private InputAction rotateViewAction;

    private Vector3 playerPosition;

    private IGrabbable grabbable;

    private GameObject activeToolObject;


    private void OnEnable()
    {
        mousePositionAction = inputActions.FindAction("Position");
        switchToolAction = inputActions.FindAction("SwitchTools");
        useToolAction = inputActions.FindAction("UseTool");
        rotateViewAction = inputActions.FindAction("RotateView");

        if (mousePositionAction != null)
        {
            mousePositionAction.performed += inputContext => UpdatePlayerPosition(inputContext);
        }

        if (switchToolAction != null)
        {
            switchToolAction.performed += inputContext => SwitchTools(inputContext);
        }

        if (useToolAction != null)
        {
            useToolAction.performed += _ => OnUseTool();
        }
    }

    private void OnUseTool()
    {
        Debug.Log("Mouse left button click");
        if (activeTool != null)
        {
            activeTool.UseTool();
        }
    }

    private void Start()
    {
        playerPosition = Vector3.zero;
        handPoseController = hand.GetComponentInChildren<HandPoseController>();
    }

    private void UpdatePlayerPosition(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (input != null)
        {
            Vector3 mouseViewportPos = webGLCamera.ScreenToViewportPoint(input);
            mouseViewportPos.z = 1f;
            playerPosition = webGLCamera.ViewportToWorldPoint(mouseViewportPos);
            playerPosition.z = 0.1f;
            transform.position = playerPosition;
            //Cursor.visible = false;
        }
    }

    private void SwitchTools(InputAction.CallbackContext context)
    {
        //NOTE: Currently the only possible tool is Scissors, so I just hardcoded that for now - must change later.
        int currentIndex = 0;
        if (activeTool != null)
        {
            activeToolObject.transform.SetParent(null);
            activeTool.WebGLSwitchToDifferentTool();
            currentIndex = tools.IndexOf(activeTool.gameObject);
            if (currentIndex + 1 < tools.Count)
            {
                currentIndex++;
            }
            else
            {
                activeToolObject.transform.SetParent(null);
                handPoseController.NoTool(activeToolObject.name);
                activeToolObject = null;
                activeTool.WebGLSwitchToDifferentTool();
                activeTool = null;
                return;
            }

        }

        activeToolObject = tools[currentIndex];
        activeTool = tools[currentIndex].GetComponent<Tool>();
        GameObject activeToolAttachPoint = activeTool.toolAttachPoint;
        activeToolObject.transform.SetParent(transform);
        activeToolAttachPoint.transform.position = transform.position;
        activeTool.WebGLMakeActiveTool();
        handPoseController.HoldTool(activeToolObject.name);
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

        if (useToolAction != null)
        {
            useToolAction.performed -= _ => OnUseTool();
        }
    }
}
