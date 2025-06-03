using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapsManager : MonoBehaviour
{
    public Text timerText;             // Timer text
    public Text GameOverText;          // Fail text
    public Text winMessageText;        // Success text
    public Text lapTrackText;          // Lap tracker text
    public float startCountdown = 180f;
    public int totalLaps = 3;
    private int currentLap = 0;
    public PlayerController_updated playerController;
    public EnemyManagers enemyManager;
    public GameObject ResultMenuCanvas;
    public GameObject WinPanel;
    public GameObject GameOverPanel;


    private bool isRunning = false;
    private float countdownTime;
    public bool hasPassedCheckpoint = false;

    void Start()
    {
        Debug.Log("Time.timeScale: " + Time.timeScale);
        countdownTime = startCountdown;
        UpdateTimerDisplay(countdownTime);

        GameOverText.gameObject.SetActive(false); // Hide fail text at start
        winMessageText.gameObject.SetActive(false); // Hide success text at start
        currentLap = 0;
        UpdateLapText();
        Debug.Log("S1 Scene Started");
        //StartTimer(); // Optional: start automatically
    }

    void Update()
    {
        if (isRunning)
        {
            countdownTime -= Time.deltaTime;

            if (countdownTime <= 0f)
            {
                countdownTime = 0f;
                isRunning = false;
                ResultMenuCanvas.SetActive(true);
                GameOverPanel.SetActive(true);
                WinPanel.SetActive(false); // Hide the other panel
                GameOverText.gameObject.SetActive(true); // Ensure text object is enabled
                GameOverText.text = "Game Over";
                playerController.enabled = false;
                Debug.Log("타이머 종료!");
            }

            UpdateTimerDisplay(countdownTime);
        }
    }

    void UpdateLapText()
    {
       lapTrackText.text = $"{currentLap}/{totalLaps}";
    }


    public void StartTimer()
    {
        if (!isRunning && countdownTime > 0f)
        {
            isRunning = true;
            Debug.Log("타이머 시작!");
        }
    }

    void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public void PlayerWins()
    {
        if (!isRunning) return; // Prevent win if time already ran out

        isRunning = false;
        ResultMenuCanvas.SetActive(true);
        WinPanel.SetActive(true);
        GameOverPanel.SetActive(false); // Hide the other panel
        winMessageText.gameObject.SetActive(true); // Ensure text object is enabled
        winMessageText.text = "You Win!";
        playerController.enabled = false;

        // 🟩 Add leaderboard saving
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int level = currentScene == "Track_2" ? 1 : 2;

        float time = startCountdown - countdownTime;
        int score = ScoreManager.score;

        LeaderboardManager.SaveToLeaderboard("Player", time, score, level);
        Debug.Log("You win!");
    }

    public void ResetTimer()
    {
        countdownTime = startCountdown;
        UpdateTimerDisplay(countdownTime);
        isRunning = true; // make sure timer keeps running
        Debug.Log("Timer Reset at Checkpoint!");
    }

    public void CompleteLap()
    {
        if (!isRunning || !hasPassedCheckpoint) return;

        currentLap++;
        hasPassedCheckpoint = false; // reset for next lap
        UpdateLapText();

        if (currentLap >= totalLaps)
        {
            PlayerWins();
        }
        else
        {
            ResetTimer(); // Reset timer for the next laps
            enemyManager?.ResetAllEnemies(); // Reset enemies when new lap starts
            Debug.Log($"Lap {currentLap} completed!");
        }
    }

}
