using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class GardenSceneUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject vrControlsUI;
    [SerializeField] private GameObject webGLControlsUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject showControlsButton;

    private InputAction pause;
    private bool openControlsFromPause;
    private Vector3 vrWorldScale = new(0.011f, 0.011f, 0.011f);
    private Vector3 vrWorldRotation = new(0, 180, 0);
    private Vector3 vrWorldPos = new(15, 5, 9);

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

        TreeTest.OnGameOver += GameOver;
    }

    private void Start()
    {
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
            vrControlsUI.SetActive(true);
        }
        else
        {
            webGLControlsUI.SetActive(true);
        }
        Time.timeScale = 0f;
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
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void ResumeGame()
    {
        AudioManager.Instance.PlaySFX("SFX_UI_ButtonHover");
        TogglePauseMenu();
    }

    public void ReturnToMainMenu()
    {
        AudioManager.Instance.PlaySFX("SFX_UI_ButtonHover");
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
        showControlsButton.SetActive(false);
    }

    public void StartOver()
    {
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
        TreeTest.OnGameOver -= GameOver;
    }
}
