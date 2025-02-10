using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GardenSceneUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject controlsGuideUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject showControlsButton;

    private InputAction pause;
    private bool openControlsFromPause;

    private void Awake()
    {
        Time.timeScale = 1f;
    }
    private void OnEnable()
    {
        GamePlatformManager.OnWebGLInitialized +=  SubscribePlayerInput;
        GamePlatformManager.OnVRInitialized += SubscribePlayerInput;

        TreeTest.OnGameOver += GameOver;
    }

    private void Start()
    {
        if (!showControlsButton.activeSelf)
        {
            showControlsButton.SetActive(true);
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
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void ResumeGame()
    {
        TogglePauseMenu();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(1);
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
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(1);
        SceneManager.LoadSceneAsync(1);
    }

    public void ToggleShowControls()
    {
        controlsGuideUI.SetActive(!controlsGuideUI.activeSelf);

        if (controlsGuideUI.activeSelf)
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
        }

    }

    private void OnDisable()
    {
        GamePlatformManager.OnWebGLInitialized -= SubscribePlayerInput;
        GamePlatformManager.OnVRInitialized -= SubscribePlayerInput;

        if (pause != null)
        {
            pause.performed -= HandleTogglePauseMenu;
        }
        TreeTest.OnGameOver -= GameOver;
    }
}
