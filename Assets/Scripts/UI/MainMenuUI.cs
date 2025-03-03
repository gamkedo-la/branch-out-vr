using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Management;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI loadingText;

    private Vector3 vrWorldScale = new(0.01f, 0.01f, 0.01f);
    private Vector3 vrWorldPos = new(-1, 4.09f, 9);

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX("SFX_UI_ButtonClick");
            LoadScene();
        });
        optionsButton.onClick.AddListener(() =>
        {
            Debug.Log("Need to add options menu");
        });
    }

    private void OnEnable()
    {
        GamePlatformManager.OnVRInitialized += SetVRUI;
    }

    private void SetVRUI()
    {
        if (GamePlatformManager.IsVRMode)
        {
            Canvas _canvas = GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.worldCamera = Camera.main;
            transform.position = vrWorldPos;
            transform.localScale = vrWorldScale;
        }
    }

    private void LoadScene()
    {
        loadingText.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
        //optionsButton.gameObject.SetActive(false);

        if (GamePlatformManager.IsVRMode)
        {
            //This means we're in a build, and should NOT do LoadVRScene, as this breaks it
            if (GamePlatformManager.Instance.isBuildVR)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                StartCoroutine(LoadEditorVRScene());
            }
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    IEnumerator LoadEditorVRScene()
    {
        Debug.Log("Stopping XR before scene switch...");
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        yield return null; // Wait a frame

        Debug.Log("Loading new scene...");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        yield return new WaitUntil(() => asyncLoad.isDone);

        Debug.Log("Restarting XR after scene switch...");
        XRGeneralSettings.Instance.Manager.InitializeLoader();
        yield return null; // Wait a frame

        XRGeneralSettings.Instance.Manager.StartSubsystems();
        Debug.Log("XR Restarted.");
    }


    private void OnDisable()
    {
        GamePlatformManager.OnVRInitialized -= SetVRUI;
    }
}
