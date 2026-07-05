using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartTimer : MonoBehaviour
{
    public static RestartTimer Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float timerReset = 3f;
    
    private bool isCounting = false;
    public bool IsCounting => isCounting;
    
    public static bool shouldCountdown = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (shouldCountdown)
        {
            shouldCountdown = false;
            Time.timeScale = 0f;
            StartCoroutine(StartCountdown());
        }
    }

    public void TriggerRestart()
    {
        Time.timeScale = 1f;
        shouldCountdown = true;

        if (TransitionManager.Instance != null)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            TransitionManager.Instance.LoadScene(currentScene);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private IEnumerator StartCountdown()
    {
        if (TransitionManager.Instance != null)
        {
            yield return new WaitForSecondsRealtime(0.2f);
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }

        isCounting = true;
        Time.timeScale = 0f;

        float timer = timerReset;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }

        while (timer > 0)
        {
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(timer).ToString();
            }

            timer -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (timerText != null)
        {
            timerText.text = "GO!";
        }

        yield return new WaitForSecondsRealtime(1f);

        if (timerText != null)
        {
            timerText.text = "";
            timerText.gameObject.SetActive(false);
        }

        Time.timeScale = 1f;
        isCounting = false;
    }
}
