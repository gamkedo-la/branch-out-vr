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

    [SerializeField]
    GameObject hand;

    private HandPoseController handPoseController;

    private InputAction mousePositionAction;

    private InputAction switchToolAction;

    private InputAction useToolAction;

    private Vector3 playerPosition;

    private GameObject activeToolObject;

    private GameObject activeToolAttachPoint;

    private float handDistanceFromCamera = 1.75f;


    private void OnEnable()
    {
        mousePositionAction = inputActions.FindAction("Position");
        switchToolAction = inputActions.FindAction("SwitchTools");
        useToolAction = inputActions.FindAction("UseTool");

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
            Vector3 mouseViewportPos = new Vector3(input.x / Screen.width, input.y / Screen.height, 1f);

            mouseViewportPos.x = (mouseViewportPos.x - 0.5f) * handDistanceFromCamera + 0.5f;
            mouseViewportPos.y = (mouseViewportPos.y - 0.5f) * handDistanceFromCamera + 0.5f;


            playerPosition = webGLCamera.ViewportToWorldPoint(mouseViewportPos);
            playerPosition.z = webGLCamera.transform.position.z - handDistanceFromCamera;

            transform.position = playerPosition; 
        }
        transform.forward = Camera.main.transform.forward;
    }

    private void SwitchTools(InputAction.CallbackContext context)
    {
        //NOTE: Currently the only possible tool is Scissors, so I just hardcoded that for now - must change later.
        int currentIndex = 0;
        if (activeTool != null)
        {
            activeToolAttachPoint.transform.localPosition = Vector3.zero;
            activeToolObject.transform.SetParent(null);
            activeTool.WebGLSwitchToDifferentTool();
            currentIndex = tools.IndexOf(activeTool.gameObject);
            if (currentIndex + 1 < tools.Count)
            {
                currentIndex++;
            }
            else
            {
                handPoseController.NoTool(activeToolObject.name);
                activeToolObject = null;
                activeTool.WebGLSwitchToDifferentTool();
                activeTool = null;
                return;
            }

        }

        activeToolObject = tools[currentIndex];
        activeTool = tools[currentIndex].GetComponent<Tool>();
        activeToolAttachPoint = activeTool.toolAttachPoint;
        activeToolObject.transform.position = transform.position;
        activeToolObject.transform.SetParent(transform, true);
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
