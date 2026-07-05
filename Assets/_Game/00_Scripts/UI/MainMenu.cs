using System.Collections;
using Slafurry.System.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string levelScene = "Game";

    [Header("UI")]
    [SerializeField] private GameObject parentPanel;
    [SerializeField] private GameObject panelCanvas;
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private GameObject guideCanvas;

    [Header("Animation")]
    [SerializeField] private GameObject panelAnimObj;
    [SerializeField] private Animator panelAnim;
    [SerializeField] private float panelAnimDuration = 0.35f;

    [Header("Cursor")]
    [SerializeField] private RectTransform cursorLock;

    private bool isTransitioning = false;

    private void Start()
    {
        Audio.PlayMusic("MainMenu");

        if (parentPanel != null)
            parentPanel.SetActive(false);

        if (panelCanvas != null)
            panelCanvas.SetActive(false);

        if (settingCanvas != null)
            settingCanvas.SetActive(false);

        if (guideCanvas != null)
            guideCanvas.SetActive(false);
    }
    private void Update()
    {
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (cursorLock == null)
            return;

        cursorLock.position = Input.mousePosition;
    }

    #region Button

    public void StartGame()
    {
        SceneManager.LoadScene(levelScene);
    }

    public void ExitGame()
    {

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion

    #region Setting

    public void OpenSetting()
    {
        if (isTransitioning)
            return;

        StartCoroutine(OpenPanel(settingCanvas));
    }

    public void CloseSetting()
    {
        if (isTransitioning)
            return;

        StartCoroutine(ClosePanel());
    }

    #endregion

    #region Guide

    public void OpenGuide()
    {
        if (isTransitioning)
            return;

        StartCoroutine(OpenPanel(guideCanvas));
    }

    public void CloseGuide()
    {
        if (isTransitioning)
            return;

        StartCoroutine(ClosePanel());
    }

    #endregion

    IEnumerator OpenPanel(GameObject targetPanel)
    {
        isTransitioning = true;


        // Aktifkan ParentPanel terlebih dahulu
        if (parentPanel != null)
            parentPanel.SetActive(true);

        // Pastikan PanelCanvas masih tersembunyi
        if (panelCanvas != null)
            panelCanvas.SetActive(false);

        if (settingCanvas != null)
            settingCanvas.SetActive(false);

        if (guideCanvas != null)
            guideCanvas.SetActive(false);

        // Mainkan animasi Open
        if (panelAnim != null)
        {
            panelAnim.Play("Open");
            yield return new WaitForSeconds(panelAnimDuration);
        }

        // Setelah animasi selesai baru tampilkan isi panel
        if (panelCanvas != null)
            panelCanvas.SetActive(true);

        if (targetPanel != null)
            targetPanel.SetActive(true);

        isTransitioning = false;
    }

    IEnumerator ClosePanel()
    {
        isTransitioning = true;


        // Hilangkan isi panel
        if (settingCanvas != null)
            settingCanvas.SetActive(false);

        if (guideCanvas != null)
            guideCanvas.SetActive(false);

        if (panelCanvas != null)
            panelCanvas.SetActive(false);

        // Animasi Close
        if (panelAnim != null)
        {
            panelAnim.Play("Close");
            yield return new WaitForSeconds(panelAnimDuration);
        }

        // Matikan parent
        if (parentPanel != null)
            parentPanel.SetActive(false);

        isTransitioning = false;
    }

}