using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;


public class SceneConfig : MonoBehaviour
{
    // Have one instance per scene that stores a reference to the relevant VR-specific and WebGL-specific objects in the scene.

    [SerializeField] private GameObject xrOrigin;
    [SerializeField] private GameObject webGL;

    private void OnEnable()
    {
        GamePlatformManager.OnPlatformDetermined += ConfigureSceneForPlatform;
    }

    private void ConfigureSceneForPlatform()
    {
        if (GamePlatformManager.Instance == null)
        {
            Debug.LogWarning("GamePlatformManager not found in the scene.");
            return;
        }
        Debug.Log("Activating objects");
        if (xrOrigin == null)
        {
            xrOrigin = GameObject.FindObjectOfType<XROrigin>(true).gameObject;
        }
        if (webGL == null)
        {
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>(true);
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i].gameObject.layer == 8)
                {
                    Debug.Log("WebGL layer found on camera");
                    webGL = cameras[i].gameObject.transform.parent.gameObject;
                }
            }
        }
        GamePlatformManager.Instance.ConfigureScene(xrOrigin, webGL);
    }
}
