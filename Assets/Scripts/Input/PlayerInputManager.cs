using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Enables input on start. Available class for other input managing funtionality as needed.
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

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
        CreateSingleton();
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
        }
        DontDestroyOnLoad(gameObject);
    }
}
