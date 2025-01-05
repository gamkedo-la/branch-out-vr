using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

/// <summary>
/// Determines if the game is running in VR or WebGL and adjusts settings and GameObjects accordingly.
/// </summary>
public class GamePlatformManager : MonoBehaviour
{
    public static bool IsVRMode { get; private set; }

    public static event Action OnVRInitialized;
    public static event Action OnWebGLInitialized;

    /// <summary>
    /// The GameObject in the scene that contains our VR rig setup.
    /// </summary>
    [SerializeField]
    GameObject xrOrigin;

    /// <summary>
    /// The camera and relevant objects/scripts for non-VR modes.
    /// </summary>
    [SerializeField]
    GameObject webGL;

    private void Start()
    {
        StartCoroutine(CheckInitializeVR());
    }

    private IEnumerator CheckInitializeVR()
    {
        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.Log("Attempting to initialize XR Loader...");
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        }

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.Log("Failed to initialize XR loader.");
            SetVRMode(false);
        }

        XRGeneralSettings.Instance.Manager.StartSubsystems();

        yield return new WaitForSeconds(1f);

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
   
    private void SetVRMode(bool enableVR)
    {
        IsVRMode = enableVR;
        if (enableVR)
        {
            Debug.Log("VR mode and device detected, enabling VR.");
            xrOrigin.SetActive(true);
            webGL.SetActive(false);
            OnVRInitialized?.Invoke();
        }

        else
        {
            Debug.Log("No VR Device. Running in \"flat\" mode.");
            xrOrigin.SetActive(false);
            webGL.SetActive(true);
            OnWebGLInitialized?.Invoke();

            //Stop and deinitialize XR if it was running.
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
        }
    }

    private void OnDisable()
    {
        if (XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }
}
