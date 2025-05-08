using UnityEngine;
using UnityEngine.InputSystem;

public class Wire : Tool, IGrabbable
{
    [SerializeField]
    private bool rotateBranchActive;

    [SerializeField] private float rotationSpeed = 50f;

    private BranchNode lastNearestNode;

    [SerializeField]
    GameObject playerHoldingToolObject;
    [SerializeField]
    private Vector3 previousPlayerPosition;

    private BranchNode currentNearestNode;

    [Header("VR input actions")]
    private InputAction rightHandPositionActionVR;
    private InputAction leftHandPositionActionVR;

    [Header("WebGL input actions")]
    private InputAction playerPositionWebGL;
    private InputAction webGLCancelToolAction;

    private void OnEnable()
    {
        GamePlatformManager.OnVRInitialized += GrabActionReferences;
        GamePlatformManager.OnWebGLInitialized += GrabActionReferences;
    }

    private void GrabActionReferences()
    {
        if (GamePlatformManager.IsVRMode)
        {
            rightHandPositionActionVR = PlayerInputManager.Instance.inputActions.FindAction("RightHandPosition");
            leftHandPositionActionVR = PlayerInputManager.Instance.inputActions.FindAction("LeftHandPosition");
        }
        else
        {
            playerPositionWebGL = PlayerInputManager.Instance.inputActions.FindAction("Position");
            webGLCancelToolAction = PlayerInputManager.Instance.inputActions.FindAction("CancelWireToolRotation");
        }
    }

    private void Start()
    {
        isActive = false;
    }

    public override void UseTool()
    {
        ToggleWireBranchNode();
    }
    public override void WebGLMakeActiveTool(GameObject currentPlayerObject)
    {
        base.WebGLMakeActiveTool();

        if (playerPositionWebGL != null)
        {
            playerPositionWebGL.performed += TrackPlayerMotion;
        }
        if (webGLCancelToolAction != null)
        {
            webGLCancelToolAction.performed += (ctx) => OnCancelRotation();
        }
        playerHoldingToolObject = currentPlayerObject;
        previousPlayerPosition = playerHoldingToolObject.transform.localPosition;
    }

    public override void WebGLSwitchToDifferentTool()
    {
        base.WebGLSwitchToDifferentTool();

        if (playerPositionWebGL != null)
        {
            playerPositionWebGL.performed -= TrackPlayerMotion;
        }
        if (webGLCancelToolAction != null)
        {
            webGLCancelToolAction.performed -= (ctx) => OnCancelRotation();
        }
        previousPlayerPosition = Vector3.zero;
    }

    private void Update()
    {
        if (isActive && ! rotateBranchActive)
        {
            currentNearestNode = ClosestBranch();

            if (lastNearestNode != currentNearestNode)
            {
                if (lastNearestNode != null)
                {
                    lastNearestNode.SetMeshRendMat(branchDefaultMat);
                }

                if (currentNearestNode != null)
                {
                    currentNearestNode.SetMeshRendMat(targetMat);
                }

                lastNearestNode = currentNearestNode;
            }
        }
    }

    private void OnDrawGizmos()
    {
        //visualize the area for cutting branches
        Gizmos.DrawSphere(proximityPoint.transform.position, overlapSphereRadius);
    }

    /// <summary>
    /// Sets rotateBranchActive to true, which allows the calling of RotateNode() to apply a rotation to the targeted node. 
    /// </summary>
    private void ToggleWireBranchNode()
    {
        if (isActive && !rotateBranchActive)
        {
            rotateBranchActive = true;
            currentNearestNode.SetStartingRotation();
        }
        else if (rotateBranchActive)
        {
            rotateBranchActive = false;
        }
    }

    /// <summary>
    /// When the wire tool is active, track the position of the player object that is holding the tool in order to determine the movement delta for rotation.
    /// </summary>
    /// <param name="context"></param>
    private void TrackPlayerMotion(InputAction.CallbackContext context)
    {
        if (playerHoldingToolObject != null)
        {
            Vector3 currentPosition = playerHoldingToolObject.transform.localPosition;
            if (currentPosition != previousPlayerPosition)
            {
                if (rotateBranchActive)
                {
                    RotateNode();
                }
                previousPlayerPosition = currentPosition;
            }
        }
    }

    /// <summary>
    /// Determines the movement delta and passes it to the targeted node to apply to its rotation.
    /// </summary>
    private void RotateNode()
    {
        if (playerHoldingToolObject != null)
        {
            if (currentNearestNode != null)
            {
                BranchNode closestBranchNode = currentNearestNode.GetComponent<BranchNode>();
                if (closestBranchNode != null)
                {
                    Vector3 currentPosition = playerHoldingToolObject.transform.localPosition;
                    Vector3 movementDelta = (currentPosition - previousPlayerPosition);

                    movementDelta = Camera.main.transform.InverseTransformDirection(movementDelta);

                    movementDelta *= rotationSpeed;

                    closestBranchNode.ApplyRotation(movementDelta);

                }
            }
        }
        else
        {
            Debug.LogError("The wire tool is missing a reference to the Player Hand object that is holding it. This is needed to track the hand's movement delta to determine the rotation to apply.");
        }
    }

    private void OnCancelRotation()
    {
        if (!rotateBranchActive) return;

        if (currentNearestNode != null)
        {
            BranchNode closestBranchNode = currentNearestNode.GetComponent<BranchNode>();
            closestBranchNode.RevertRotation();
            ToggleWireBranchNode();
        }

    }

    bool IGrabbable.CheckIfActive()
    {
        return isActive;
    }

    void IGrabbable.OnGrab(GameObject leftOrRightHand)
    {
        isActive = true;
        playerHoldingToolObject = leftOrRightHand;
        Debug.Log(playerHoldingToolObject.name);
        previousPlayerPosition = leftOrRightHand.transform.localPosition;

        if (playerHoldingToolObject.name.Contains("Right"))
        {
            rightHandPositionActionVR.performed += TrackPlayerMotion;
        }
        else if (playerHoldingToolObject.name.Contains("Left"))
        {
            leftHandPositionActionVR.performed += TrackPlayerMotion;
        }
    }

    void IGrabbable.OnRelease()
    {
        if (!GamePlatformManager.IsVRMode)
        {
            return;
        }
        //toolRB.isKinematic = false;
        isActive = false;
        transform.position = defaultTransform.transform.position;
        playerHoldingToolObject = null;
        previousPlayerPosition = Vector3.zero;
        rightHandPositionActionVR.performed -= TrackPlayerMotion;
        leftHandPositionActionVR.performed -= TrackPlayerMotion;
    }

    private void OnDisable()
    {
        if (rightHandPositionActionVR != null)
        {
            rightHandPositionActionVR.performed -= TrackPlayerMotion;
        }

        if (leftHandPositionActionVR != null)
        {
            leftHandPositionActionVR.performed -= TrackPlayerMotion;
        }

        if (playerPositionWebGL != null)
        {
            playerPositionWebGL.performed -= TrackPlayerMotion;
        }

        if (webGLCancelToolAction != null)
        {
            webGLCancelToolAction.performed -= (ctx) => OnCancelRotation();
        }
    }
}
