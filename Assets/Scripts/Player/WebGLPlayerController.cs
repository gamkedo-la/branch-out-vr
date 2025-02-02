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
    private List<GameObject> tools;

    [SerializeField]
    GameObject hand;

    public float depthOffset=0.0f;

    private HandPoseController handPoseController;

    private InputAction mousePositionAction;

    private InputAction switchToolAction;

    private InputAction useToolAction;

    private Vector3 playerPosition;

    private GameObject activeToolObject;

    private GameObject activeToolAttachPoint;

    private float handDistanceFromCamera = 1.75f;

    private InputActionAsset inputActions;

    private void OnEnable()
    {
        inputActions = PlayerInputManager.Instance.inputActions;
        mousePositionAction = inputActions.FindAction("Position");
        switchToolAction = inputActions.FindAction("SwitchTools");
        useToolAction = inputActions.FindAction("UseTool");
        WebGLCameraController.OnCameraViewRotated += UpdatePlayerRotation;

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


            Vector3 worldPosition = webGLCamera.ViewportToWorldPoint(mouseViewportPos);

            Vector3 cameraRight = webGLCamera.transform.right;
            Vector3 cameraUp = webGLCamera.transform.up;

            Vector3 cameraForward = webGLCamera.transform.forward;
            Vector3 cameraOrigin = webGLCamera.transform.position;

            Vector3 offsetFromCamera = (worldPosition - cameraOrigin);
            
            playerPosition = cameraOrigin + cameraForward * handDistanceFromCamera + Vector3.Dot(offsetFromCamera, cameraRight) * cameraRight + Vector3.Dot(offsetFromCamera, cameraUp) * cameraUp;

            transform.position = playerPosition + depthOffset * cameraForward;
        }

    }

    private void UpdatePlayerRotation()
    {
        transform.forward = Camera.main.transform.forward;
        if (activeToolObject != null)
        {
            activeToolObject.transform.forward = transform.forward;
        }
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
        activeToolObject.transform.position = hand.transform.position;
        activeToolObject.transform.SetParent(transform, true);
        activeToolAttachPoint.transform.position = hand.transform.position;
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
        WebGLCameraController.OnCameraViewRotated -= UpdatePlayerRotation;

    }
}
