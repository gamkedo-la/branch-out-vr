using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputHandler _instance;
    public static PlayerInputHandler Instance { get; private set; }
    [SerializeField]
    public InputActionAsset inputActions;

    private InputActionMap vrInputActions;

    private void OnEnable()
    {
        inputActions.Enable();
        vrInputActions = inputActions.actionMaps.First((map) => map.name.Contains("VR"));
        if (vrInputActions == null)
        {
            Debug.LogError("No action map associated with VR movement tracking. Make sure the name of the action map contains VR.");
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
