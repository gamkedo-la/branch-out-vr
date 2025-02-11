using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneConfig : MonoBehaviour
{
    // Have one instance per scene that stores a reference to the relevant VR-specific and WebGL-specific objects in the scene.

    [SerializeField] private GameObject xrOrigin;
    [SerializeField] private GameObject webGL;

    private void Start()
    {
        if (GamePlatformManager.Instance == null)
        {
            Debug.LogWarning("GamePlatformManager not found in the scene.");
            return;
        }

        GamePlatformManager.Instance.ConfigureScene(xrOrigin, webGL);
    }
}
