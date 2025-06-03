using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject entryTemplate;
    public Transform entryContainer;
    public Button backButton;
    public Button resetButton;

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public float time;
        public int score;     // score from ScoreManager
        public int level;
    }

    [System.Serializable]
    public class Leaderboard
    {
        public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    }

    void Start()
    {
        ShowLeaderboard();

        backButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu"); // Replace with your actual main menu scene name
        });

        resetButton.onClick.AddListener(() => {
            PlayerPrefs.DeleteKey("leaderboard");
            foreach (Transform child in entryContainer)
            {
                Destroy(child.gameObject);
            }
        });
    }

    void ShowLeaderboard()
    {
        string json = PlayerPrefs.GetString("leaderboard", "{}");
        Leaderboard leaderboard = JsonUtility.FromJson<Leaderboard>(json);

        // Sort entries by time descending
        leaderboard.entries.Sort((a, b) => b.time.CompareTo(a.time));

        for (int i = 0; i < leaderboard.entries.Count; i++)
        {

            float time = leaderboard.entries[i].time;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
            string timeFormatted = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);

            GameObject entry = Instantiate(entryTemplate, entryContainer);
            entry.SetActive(true);
            Text text = entry.GetComponent<Text>();
            text.text = $"{i + 1}. {leaderboard.entries[i].playerName} - Time: {leaderboard.entries[i].time:F2} - Score: {leaderboard.entries[i].score} - Level {leaderboard.entries[i].level}";
        }
    }

    public static void SaveToLeaderboard(string playerName, float time, int score, int level)
    {
        string json = PlayerPrefs.GetString("leaderboard", "{}");
        Leaderboard leaderboard = JsonUtility.FromJson<Leaderboard>(json);

        LeaderboardEntry newEntry = new LeaderboardEntry
        {
            playerName = playerName,
            time = time,
            score = score,
            level = level
        };

        leaderboard.entries.Add(newEntry);

        string newJson = JsonUtility.ToJson(leaderboard);
        PlayerPrefs.SetString("leaderboard", newJson);
        PlayerPrefs.Save();
    }
}
