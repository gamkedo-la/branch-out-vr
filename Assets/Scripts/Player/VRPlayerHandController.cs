using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class VRPlayerHandController : MonoBehaviour
{
    [Header("Shown here for debugging"), Space]
    public Tool activeTool;

    [SerializeField]
    private GameObject heldObject;

    [SerializeField, Header("Regular serializable fields"), Space]
    bool isLeftHand = false;

    [SerializeField]
    GameObject heldItemAttachPoint;

    [SerializeField]
    private IGrabbable grabbableInRange;

    [SerializeField]
    InputActionAsset inputActions;

    InputAction grabAction;

    InputAction useToolAction;

    UnityEngine.XR.InputDevice hapticDevice;

    private void OnEnable()
    {
        Debug.Log("On enable.");
        GamePlatformManager.OnVRInitialized += TryGetHapticDevice;

        grabAction = inputActions.FindAction("Grab");
        useToolAction = inputActions.FindAction("UseTool");

        grabAction.performed += _ => Grab();
        grabAction.canceled += _ => Release();
        useToolAction.performed += _ => UseTool();
    }

    private void Start()
    {
        if (!isLeftHand)
        {
            Debug.Log("looking for haptic.");
            hapticDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }
        else
        {
            hapticDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }
        Debug.Log("haptic device: " + hapticDevice.name);
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

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<IGrabbable>(out IGrabbable grabbable);
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
    private void Grab()
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
                    grabbableInRange.OnGrab();
                }
            }
            if (heldObject != null)
            {
                Debug.Log("Setting position and rotation of tool relative to hand.");
                heldObject.transform.SetParent(heldItemAttachPoint.transform);
                heldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
    }

    private void Release()
    {
        if (grabbableInRange != null)
        {
            grabbableInRange.OnRelease();
        }

        if (heldObject != null)
        {
            Debug.Log("Release held object.");
            heldObject.GetComponent<IGrabbable>().OnRelease();
            if (activeTool != null)
            {
                activeTool = null;
            }
            heldObject.transform.SetParent(null);
            heldObject = null;
        }
    }

    private void UseTool()
    {
        if (activeTool != null)
        {
            activeTool.UseTool();
        }
    }

    private void OnDisable()
    {
        GamePlatformManager.OnVRInitialized -= TryGetHapticDevice;

        grabAction.performed -= _ => Grab();
        grabAction.canceled -= _ => Release();
        useToolAction.performed -= _ => UseTool();
    }
}