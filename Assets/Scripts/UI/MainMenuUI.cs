using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    private Vector3 vrWorldScale = new(0.01f, 0.01f, 0.01f);
    private Vector3 vrWorldPos = new(-1, 5, 9);

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
        optionsButton.onClick.AddListener(() =>
        {
            Debug.Log("Need to add options menu");
        });
        exitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }

    private void Start()
    {
        if (GamePlatformManager.IsVRMode)
        {
            Canvas _canvas = GetComponentInParent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.worldCamera = Camera.main;
            transform.position = vrWorldPos;
            transform.localScale = vrWorldScale;
        }
    }
}
