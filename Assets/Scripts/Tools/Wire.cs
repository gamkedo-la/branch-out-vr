using UnityEngine;
using UnityEngine.InputSystem;

public class Wire : Tool, IGrabbable
{
    [SerializeField]
    private bool rotateBranchActive;

    [SerializeField] private float rotationSpeed = 50f;

    private TreePart lastNearestBranch;

    [SerializeField]
    GameObject playerHoldingToolObject;
    [SerializeField]
    private Vector3 previousPlayerPosition;

    private TreePart closestBranch;

    [Header("VR input actions to track movement")]
    private InputAction rightHandPositionActionVR;
    private InputAction leftHandPositionActionVR;

    [Header("WebGL input action to track movement")]
    private InputAction playerPositionWebGL;

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
        }
    }

    private void Start()
    {
        isActive = false;
    }

    public override void UseTool()
    {
        WireBranchNode();
    }
    public override void WebGLMakeActiveTool(GameObject currentPlayerObject)
    {
        base.WebGLMakeActiveTool();

        if (playerPositionWebGL != null)
        {
            playerPositionWebGL.performed += TrackPlayerMotion;
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
        previousPlayerPosition = Vector3.zero;
    }

    private void Update()
    {
        if (isActive && ! rotateBranchActive)
        {
            closestBranch = ClosestBranch();

            if (lastNearestBranch != closestBranch)
            {
                if (lastNearestBranch != null)
                {
                    BranchNode lastAffectedNode = lastNearestBranch.GetComponent<BranchNode>();
                    lastAffectedNode.SetMeshRendMat(branchDefaultMat);
                }

                if (closestBranch != null)
                {
                    BranchNode currentAffectedNode = closestBranch.GetComponent<BranchNode>();
                    currentAffectedNode.SetMeshRendMat(targetMat);
                }

                lastNearestBranch = closestBranch;
            }
        }
    }

    private void OnDrawGizmos()
    {
        //visualize the area for cutting branches
        Gizmos.DrawSphere(proximityPoint.transform.position, overlapSphereRadius);
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
            Debug.Log(currentPosition + " current position, " + previousPlayerPosition + " previous position");
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
        Debug.Log("Rotate node");
        if (playerHoldingToolObject != null)
        {
            if (closestBranch != null)
            {
                BranchNode closestBranchNode = closestBranch.GetComponent<BranchNode>();
                if (closestBranchNode != null)
                {
                    Vector3 currentPosition = playerHoldingToolObject.transform.localPosition;
                    Vector3 movementDelta = (currentPosition - previousPlayerPosition) * rotationSpeed;
                    closestBranchNode.ApplyRotation(movementDelta);
                }
            }
        }
        else
        {
            Debug.LogError("The wire tool is missing a reference to the Player Hand object that is holding it. This is needed to track the hand's movement delta to determine the rotation to apply.");
        }
    }

    /// <summary>
    /// Sets rotateBranchActive to true, which allows the calling of RotateNode() to apply a rotation to the targeted node. 
    /// </summary>
    private void WireBranchNode()
    {
        if (isActive && !rotateBranchActive)
        {
            rotateBranchActive = true;
        }
        else if (rotateBranchActive)
        {
            rotateBranchActive = false;
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
    }
}
