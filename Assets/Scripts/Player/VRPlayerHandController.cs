using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRPlayerHandController : MonoBehaviour
{
    [Header("Shown here for debugging"), Space]
    public Tool activeTool;

    [SerializeField]
    private GameObject heldObject;

    [SerializeField, Header("Regular serializable fields"), Space]
    bool isLeftHand = false;

    [SerializeField]
    GameObject handAttachPoint;

    [SerializeField]
    private IGrabbable grabbableInRange;

    [SerializeField]
    private HandPoseController handPoseController;

    private InputActionAsset inputActions;

    private InputAction grabAction;

    private InputAction useToolAction;

    private UnityEngine.XR.InputDevice hapticDevice;

    private void Start()
    {
        inputActions = PlayerInputManager.Instance.inputActions;
        GamePlatformManager.OnVRInitialized += TryGetHapticDevice;

        if (!isLeftHand)
        {
            Debug.Log("looking for haptic.");
            grabAction = inputActions.FindAction("RightHandGrab");
            useToolAction = inputActions.FindAction("RightHandUseTool");
            hapticDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }
        else
        {
            grabAction = inputActions.FindAction("LeftHandGrab");
            useToolAction = inputActions.FindAction("LeftHandUseTool");
            hapticDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }
        Debug.Log("haptic device: " + hapticDevice.name);

        handPoseController = GetComponentInChildren<HandPoseController>();

        grabAction.performed += Grab;
        grabAction.canceled += Release;
        useToolAction.performed += UseTool;
    }

    private void TryGetHapticDevice()
    {
        if (hapticDevice == null)
        {
            if (!isLeftHand)
            {
                hapticDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            }
            else
            {
                hapticDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            }
        }


        if (hapticDevice.isValid)
        {
            Debug.Log("Haptic device found successfully.");
        }
        else
        {
            Debug.LogWarning("Failed to find haptic device.");
        }
    }

    private void Update()
    {
/*        if (!isLeftHand)
        {
            PointerEventData eventData = new(EventSystem.current);
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                Debug.Log("UI HIT: " + result.gameObject.name);
            }
        }*/

    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<IGrabbable>(out IGrabbable grabbable);
        Debug.Log("Here");
        if (grabbable != null)
        {
            Debug.Log("Found grabbable object in range on trigger enter.");
            grabbableInRange = grabbable;
            Debug.Log(grabbableInRange.CheckIfActive());
            if (!grabbableInRange.CheckIfActive())
            {
                Debug.Log("Grabbable is not already active.");
                hapticDevice.SendHapticImpulse(0, 0.5f, 0.1f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (grabbableInRange != null)
        {
            other.TryGetComponent<IGrabbable>(out IGrabbable grabbable);
            if (grabbable == grabbableInRange)
            {
                grabbableInRange = null;
            }
        }
    }
    private void Grab(InputAction.CallbackContext context)
    {
        if (grabbableInRange != null)
        {
            if (grabbableInRange is Tool tool)
            {
                Debug.Log("Grabbable in range is of type tool. Setting as active tool.");
                if (!tool.isActive)
                {
                    activeTool = tool;
                    heldObject = activeTool.gameObject;
                    handPoseController.HoldTool(heldObject.name);
                    grabbableInRange.OnGrab(gameObject);
                }
            }
            if (heldObject != null)
            {
                Debug.Log("Setting position and rotation of tool relative to hand.");
                heldObject.transform.SetParent(handAttachPoint.transform);

                heldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            }
        }
    }

    private void Release(InputAction.CallbackContext context)
    {
        grabbableInRange?.OnRelease();

        if (heldObject != null)
        {
            Debug.Log("Release held object.");
            handPoseController.NoTool(heldObject.name);
            heldObject.GetComponent<IGrabbable>().OnRelease();
            if (activeTool != null)
            {
                activeTool = null;
            }
            heldObject.transform.SetParent(null);
            heldObject = null;
        }
    }

    private void UseTool(InputAction.CallbackContext context)
    {
        if (activeTool != null)
        {
            activeTool.UseTool();
        }
    }

    private void OnDisable()
    {
        GamePlatformManager.OnVRInitialized -= TryGetHapticDevice;

        if (grabAction != null)
        {
            grabAction.performed -= Grab;
            grabAction.canceled -= Release;
        }

        if (useToolAction != null)
        {
            useToolAction.performed -= UseTool;
        }
    }
}
