using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class VRStatus : MonoBehaviour
{
    public bool isVR;
    [SerializeField]
    GameObject vr;

    [SerializeField]
    GameObject webGL;

    [SerializeField]
    InputActionAsset vrInput;

    [SerializeField]
    InputActionAsset webGLInput;
    private void Awake()
    {
/*#if UNITY_EDITOR
        var deviceSimulator = FindObjectOfType<XRDeviceSimulator>();
        if (deviceSimulator != null)
        {
            isVR = true;
            vr.SetActive(true);
            webGL.SetActive(false);
            vrInput.Enable();
        }
        else
        {
            vr.SetActive(false);
            webGL.SetActive(true);
            webGLInput.Enable();
        }
#endif
        if (Application.platform == RuntimePlatform.Android)
        {
            CheckXRStatus();
            isVR = true;
            vr.SetActive(true);
            webGL.SetActive(false);
            vrInput.Enable();
            //set vr related objects active
        }
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            CheckXRStatus();
            isVR = false;
            vr.SetActive(false);
            webGL.SetActive(true);
            webGLInput.Enable();
            //set webgl related objects active
        }
*/    }
    public void CheckXRStatus()
    {
        if (UnityEngine.XR.XRSettings.enabled)
        {
            Debug.Log("XR is active");
        }
        else
        {
            Debug.Log("XR not active; using WebGL");
        }
    }


}
