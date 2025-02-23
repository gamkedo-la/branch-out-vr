using System;
using System.Collections;
using Unity.XR.CoreUtils;
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
        if (!hasCheckedPlatform)
        {
            StartCoroutine(CheckInitializeVR());
        }
    }

    private IEnumerator CheckInitializeVR()
    {
        hasCheckedPlatform = true;

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.Log(XRGeneralSettings.Instance.Manager.activeLoaders.Count + " active XR loaders");
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

        //yield return new WaitForSeconds(1f);

        if (XRSettings.isDeviceActive)
        {
            SetVRMode(true);
        }

        else
        {
            Debug.Log("No VR device detected.");
            SetVRMode(false);
        }
    }

    private void OnSceneChanged(Scene currentScene, Scene nextScene)
    {
        Debug.Log($"Scene changed to: {nextScene.name}");
        OnPlatformDetermined?.Invoke();
    }

    public void ConfigureScene(GameObject xr, GameObject webGL)
    {

        if (IsVRMode)
        {
            Debug.Log("VR mode and device detected, enabling VR.");

            if (xr != null) xr.SetActive(true);
            Debug.Log(Camera.main.name);
            if (webGL != null) webGL.SetActive(false);
            OnVRInitialized?.Invoke();
        }
        else
        {
            Debug.Log("No VR Device. Running in \"flat\" mode.");
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
        Debug.Log("Set VR Mode");
        IsVRMode = enableVR;
        OnPlatformDetermined?.Invoke();
    }

    private void OnDisable()
    {
        if (XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
}
