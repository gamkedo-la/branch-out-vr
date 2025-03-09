using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GardenSceneUI : MonoBehaviour
{
    public static event Action OnPauseGame;
    public static event Action OnResumeGame;
    public bool isGamePaused;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject vrControlsUI;
    [SerializeField] private GameObject webGLControlsUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject winGameUI;
    [SerializeField] private GameObject showControlsButton;

    private InputAction pause;
    private bool openControlsFromPause;
    private Vector3 vrWorldScale = new(0.0022f, 0.0022f, 0.0022f);
    private Vector3 vrWorldRotation = new(0, 180, 0);
    private Vector3 vrWorldPos = new(15.074f, 4.3f, 9.021f);
    private Vector2 widthHeight = new(800, 555);

    private float delayGameOverAmount = 0.5f;

    private void Awake()
    {
        Time.timeScale = 1f;
    }
    private void OnEnable()
    {
        GamePlatformManager.OnWebGLInitialized +=  SubscribePlayerInput;
        GamePlatformManager.OnVRInitialized += SubscribePlayerInput;
        GamePlatformManager.OnVRInitialized += SetPlatformUI;
        GamePlatformManager.OnWebGLInitialized += SetPlatformUI;

        ProceduralTree.OnGameOver += GameOver;
        ProceduralTree.OnGameWin += GameWin;
    }

    private void Start()
    {
        isGamePaused = false;
        if (!showControlsButton.activeSelf)
        {
            showControlsButton.SetActive(true);
        }
    }

    private void SetPlatformUI()
    {
        if (GamePlatformManager.IsVRMode)
        {
            Canvas _canvas = GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.worldCamera = Camera.main;
            transform.position = vrWorldPos;
            transform.localScale = vrWorldScale;
            transform.rotation = Quaternion.Euler(vrWorldRotation);
            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = widthHeight;
            if (!GamePlatformManager.Instance.controlsGuideShownAtStart)
            {
                vrControlsUI.SetActive(true);
                Time.timeScale = 0f;
                GamePlatformManager.Instance.controlsGuideShownAtStart = true;
            }
        }
        else
        {
            if (!GamePlatformManager.Instance.controlsGuideShownAtStart)
            {
                webGLControlsUI.SetActive(true);
                Time.timeScale = 0f;
                GamePlatformManager.Instance.controlsGuideShownAtStart = true;
            }
        }
    }

    private void SubscribePlayerInput()
    {
        pause = PlayerInputManager.Instance.inputActions.FindAction("OpenPauseMenu");
        if (pause != null)
        {
            pause.performed += HandleTogglePauseMenu;
        }
    }

    private void HandleTogglePauseMenu(InputAction.CallbackContext context)
    {
        TogglePauseMenu();
    }

    private void TogglePauseMenu()
    {
        pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);
        if (pauseMenuUI.activeSelf)
        {
            Time.timeScale = 0f;
            isGamePaused = true;
            OnPauseGame?.Invoke();
        }
        else
        {
            Time.timeScale = 1f;
            isGamePaused = false;
            OnResumeGame?.Invoke();
        }
    }

    public void ResumeGame()
    {
        OnResumeGame?.Invoke();
        Time.timeScale = 1f;
        AudioManager.Instance.PlaySFX("SFX_UI_ButtonHover");
        TogglePauseMenu();
    }

    public void ReturnToMainMenu()
    {
        AudioManager.Instance.PlaySFX("SFX_UI_ButtonHover");
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    public void GameWin()
    {
        isGamePaused = true;
        AudioManager.Instance.PlaySFX("SFX_Tree_Gleaming");
        winGameUI.SetActive(true);
    }

    public void ContinuePlaying()
    {
        AudioManager.Instance.PlaySFX("SFX_UI_ButtonHover");
        Time.timeScale = 1f;
        winGameUI.SetActive(false);
        OnResumeGame?.Invoke();
    }

    public void GameOver()
    {
        StartCoroutine(DelayGameOver());
    }

    private IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(delayGameOverAmount);
        isGamePaused = true;
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
        showControlsButton.SetActive(false);
    }

    public void StartOver()
    {
        isGamePaused = false;
        AudioManager.Instance.PlaySFX("SFX_UI_ButtonClick");
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(1);
    }

    public void ToggleShowControls()
    {
        bool justActivated;
        AudioManager.Instance.PlaySFX("SFX_UI_ButtonHover");
        if (GamePlatformManager.IsVRMode)
        {
            vrControlsUI.SetActive(!vrControlsUI.activeSelf);
            justActivated = vrControlsUI.activeSelf;
        }
        else
        {
            webGLControlsUI.SetActive(!webGLControlsUI.activeSelf);
            justActivated = webGLControlsUI.activeSelf;
        }

        if (justActivated)
        {
            if (pauseMenuUI.activeSelf)
            {
                openControlsFromPause = true;
                pauseMenuUI.SetActive(false);
            }
        }

        else
        {
            if (openControlsFromPause)
            {
                openControlsFromPause = false;
                pauseMenuUI.SetActive(true);
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }

    public void EasyCloseControlsGuide()
    {
        if (webGLControlsUI.activeSelf)
        {
            webGLControlsUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void OnDisable()
    {
        GamePlatformManager.OnWebGLInitialized -= SubscribePlayerInput;
        GamePlatformManager.OnVRInitialized -= SubscribePlayerInput;
        GamePlatformManager.OnVRInitialized -= SetPlatformUI;
        GamePlatformManager.OnWebGLInitialized -= SetPlatformUI;


        if (pause != null)
        {
            pause.performed -= HandleTogglePauseMenu;
        }
        ProceduralTree.OnGameOver -= GameOver;
        ProceduralTree.OnGameWin -= GameWin;
    }
}
