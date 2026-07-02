using System.Collections;
using Game.Manager;
using Game.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Header("Game")]
    [SerializeField] private string levelScene = "MainMenu";
    [SerializeField] private string restartScene = "IntroCutscene";
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject respawnPos;
    [SerializeField] private TextMeshProUGUI threatPoint;
    [SerializeField] private TextMeshProUGUI growthPoint;
    [SerializeField] private TextMeshProUGUI SubscriberPoint;

    [Header("Player Data")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerGrowth playerGrowth;
    [SerializeField] private ThreatPointManager playerThreath;
    [SerializeField] private SubscriptionPointManager playerSubs;
    // [SerializeField] private PlayerGrowth playerGrowth;

    [Header("Settings")]
    [SerializeField] private float freezeDuration = 2f;

    private bool gameOver = false;

    private void Start()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        if (playerHealth != null)
        {
            playerHealth.OnDied += HandlePlayerDied;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDied -= HandlePlayerDied;
        }
    }

    private void HandlePlayerDied()
    {
        TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        if (gameOver)
            return;

        gameOver = true;
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(freezeDuration);

        if (threatPoint != null && playerThreath != null)
        {
            threatPoint.text = playerThreath.GetPoint() + " Threat Point";
        }

        if (SubscriberPoint != null && playerSubs != null)
        {
            SubscriberPoint.text = playerSubs.GetPoint() + " Subscription";
        }

        if (growthPoint != null && playerGrowth != null)
        {
            growthPoint.text = playerGrowth.GetCurrentGrowPoints() + " Growth";
        }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(restartScene))
        {
            Debug.LogWarning("Restart scene name belum diisi.");
            return;
        }

        if (TransitionManager.Instance != null)
            TransitionManager.Instance.LoadScene(restartScene);
        else
            SceneManager.LoadScene(restartScene);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(levelScene))
        {
            Debug.LogWarning("Level scene name belum diisi.");
            return;
        }

        if (TransitionManager.Instance != null)
            TransitionManager.Instance.LoadScene(levelScene);
        else
            SceneManager.LoadScene(levelScene);
    }
}