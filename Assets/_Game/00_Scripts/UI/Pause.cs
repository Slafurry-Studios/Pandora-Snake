using System.Collections;
using Slafurry.System.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameSceneName = "IntroCutscene";

    [Header("UI")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingPause;
    [SerializeField] private GameObject buttonPause;
    [SerializeField] private GameObject panelUI;

    [Header("Animation")]
    [SerializeField] private Animator panelAnim;
    [SerializeField] private GameObject panelAnimObj;
    [SerializeField] private float panelAnimDuration = 0.35f;

    [Header("Cursor")]
    [SerializeField] private RectTransform cursorLock;

    private bool isPaused = false;
    private bool isTransitioning = false;

    private void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (settingPause != null)
            settingPause.SetActive(false);

        if (panelUI != null)
            panelUI.SetActive(false);

        if (buttonPause != null)
            buttonPause.SetActive(false);

        if (panelAnimObj != null)
            panelAnimObj.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        UpdateCursor();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isTransitioning)
                return;

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void UpdateCursor()
    {
        if (cursorLock == null)
            return;

        cursorLock.position = Input.mousePosition;
    }

    public void PauseGame()
    {
        if (isPaused || isTransitioning)
            return;

        StartCoroutine(PauseRoutine());
    }

    private IEnumerator PauseRoutine()
    {
        isTransitioning = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        if (panelAnimObj != null)
            panelAnimObj.SetActive(true);

        if (panelAnim != null)
        {
            panelAnim.Play("Open");
            yield return new WaitForSecondsRealtime(panelAnimDuration);
        }

        if (panelUI != null)
            panelUI.SetActive(true);

        if (buttonPause != null)
            buttonPause.SetActive(true);

        Time.timeScale = 0f;

        isPaused = true;
        isTransitioning = false;
    }

    public void ResumeGame()
    {
        if (!isPaused || isTransitioning)
            return;

        StartCoroutine(ResumeRoutine());
    }

    private IEnumerator ResumeRoutine()
    {
        isTransitioning = true;


        Time.timeScale = 1f;

        if (panelUI != null)
            panelUI.SetActive(false);

        if (buttonPause != null)
            buttonPause.SetActive(false);

        if (settingPause != null)
            settingPause.SetActive(false);

        if (panelAnim != null)
        {
            panelAnim.Play("Close");
            yield return new WaitForSecondsRealtime(panelAnimDuration);
        }

        if (panelAnimObj != null)
            panelAnimObj.SetActive(false);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        isPaused = false;
        isTransitioning = false;
    }

    public void OpenSetting()
    {

        if (settingPause != null)
            settingPause.SetActive(true);

        if (buttonPause != null)
            buttonPause.SetActive(false);

        if (panelUI != null)
            panelUI.SetActive(false);
    }

    public void CloseSetting()
    {

        if (settingPause != null)
            settingPause.SetActive(false);

        if (buttonPause != null)
            buttonPause.SetActive(true);

        if (panelUI != null)
            panelUI.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogWarning("Main menu scene name belum diisi.");
            return;
        }

        SceneSystem.Load(mainMenuSceneName);
    }

    public void RestartGame()
    {
        if (RestartTimer.Instance != null)
        {
            RestartTimer.Instance.TriggerRestart();
        }
        else
        {
            Debug.LogWarning("RestartTimer Instance not found!");
            Time.timeScale = 1f;

            SceneSystem.Load(gameSceneName);
        }
    }
}