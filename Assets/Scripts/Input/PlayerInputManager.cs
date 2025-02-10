using System.Collections.Generic;
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

    private void Awake()
    {
        CreateSingleton();
        inputActions.Enable();
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
