using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Management;

/// <summary>
/// Determines if the game is running in VR or WebGL and adjusts settings and GameObjects accordingly.
/// </summary>
public class GamePlatformManager : MonoBehaviour
{
    public static GamePlatformManager Instance;
    public static bool IsVRMode { get; private set; }

    public static event Action OnPlatformDetermined;
    public static event Action OnVRInitialized;
    public static event Action OnWebGLInitialized;

    private bool hasCheckedPlatform = false;

    /// <summary>
    /// When true, this will set all platform-specific rules and objects for a build rather than determine them at startup. 
    /// </summary>
    public bool setPlatformForBuild = false;

    /// <summary>
    /// If setPlatformForBuild is true, set this bool to true to use VR platform-specific rules and objects, and false to use WebGL. 
    /// </summary>
    public bool isBuildVR = false;

    public bool controlsGuideShownAtStart = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
    }

    private void Start()
    {
        if (setPlatformForBuild)
        {
            SetVRMode(isBuildVR);
            OnPlatformDetermined?.Invoke();
        }
        else
        {
            if (!hasCheckedPlatform)
            {
                StartCoroutine(CheckInitializeVR());
            }
        }
    }

    private IEnumerator CheckInitializeVR()
    {
        hasCheckedPlatform = true;

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.Log("Attempting to initialize XR Loader...");
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.Log("Failed to initialize XR loader.");
                SetVRMode(false);
                yield break;
            }
        }

        Debug.Log($"XR Loader initialized: {XRGeneralSettings.Instance.Manager.activeLoader.name}");

        XRGeneralSettings.Instance.Manager.StartSubsystems();

        if (XRSettings.isDeviceActive)
        {
            Debug.Log("VR device found. Enabling VR mode.");
            SetVRMode(true);
        }

        else
        {
            Debug.Log("No VR device detected. Running in flat mode.");
            SetVRMode(false);
        }
    }

    private void OnSceneChanged(Scene currentScene, Scene nextScene)
    {
        if (!setPlatformForBuild)
        {
            StartCoroutine(CheckInitializeVR());
        }
        else
        {
            SetVRMode(isBuildVR);
            OnPlatformDetermined?.Invoke();
        }
    }

    public void ConfigureScene(GameObject xr, GameObject webGL)
    {

        if (IsVRMode)
        {
            Debug.Log("Configure scene for VR.");
            if (xr != null) xr.SetActive(true);
            if (webGL != null) webGL.SetActive(false);
            OnVRInitialized?.Invoke();
        }
        else
        {
            if (xr != null) xr.SetActive(false);
            if (webGL != null) webGL.SetActive(true);
            OnWebGLInitialized?.Invoke();

            //Stop and deinitialize XR if it was running.
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
        }
    }

    private void SetVRMode(bool enableVR)
    {
        Debug.Log("Set VR Mode to " + enableVR);
        IsVRMode = enableVR;
        OnPlatformDetermined?.Invoke();
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
}
