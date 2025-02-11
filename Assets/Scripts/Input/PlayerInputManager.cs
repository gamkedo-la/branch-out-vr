using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

/// <summary>
/// Enables input on start. Available class for other input managing funtionality as needed.
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    public InputActionAsset inputActions;

    [SerializeField] private InputSystemUIInputModule uiInputWebGL;
    [SerializeField] private XRUIInputModule uiInputVR;

    private void Awake()
    {
        CreateSingleton();
        inputActions.Enable();
        uiInputWebGL = GetComponent<InputSystemUIInputModule>();
        uiInputVR = GetComponent<XRUIInputModule>();
        GamePlatformManager.OnVRInitialized += SetActiveUIEvents;
        GamePlatformManager.OnWebGLInitialized += SetActiveUIEvents;
    }

    private void SetActiveUIEvents()
    {
        if (GamePlatformManager.IsVRMode)
        {
            uiInputVR.enabled = true;
            uiInputWebGL.enabled = false;
        }
        else
        {
            uiInputVR.enabled = false;
            uiInputWebGL.enabled = true;
        }
    }

    private void CreateSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
