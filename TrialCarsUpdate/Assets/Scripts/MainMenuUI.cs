using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{

    public GameObject mainMenuUI;
    public AudioSource mainMenuMusic;
    public Light mainLight;

    void Start()
    {
        // Load saved music volume
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (mainMenuMusic != null)
        {
            mainMenuMusic.volume = savedVolume;
        }
        else
        {
            Debug.LogWarning("MainMenuMusic AudioSource not assigned.");
        }

        // Load saved resolution
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", -1);
        if (savedResolutionIndex > 0)
        {
            Resolution[] resolutions = Screen.resolutions;
            if (savedResolutionIndex - 1 < resolutions.Length)
            {
                Resolution res = resolutions[savedResolutionIndex - 1];
                Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);
                Debug.Log("Resolution loaded: " + res.width + "x" + res.height);
            }
        }

        // Load and apply brightness if light exists
        if (mainLight != null)
        {
            float savedBrightness = PlayerPrefs.GetFloat("Brightness", 1f);
            mainLight.intensity = savedBrightness;
            Debug.Log("Brightness loaded: " + savedBrightness);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SelectLevel"); // select level screen
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }

    public void LeaderboardGame()
    {
        StartCoroutine(LoadLeaderboardWithSoundDelay()); // leaderboard screen
    }

    public void OptionGame()
    {
        StartCoroutine(LoadOptionsWithSoundDelay());
    }

    private IEnumerator LoadOptionsWithSoundDelay()
    {
        yield return new WaitForSeconds(0.2f); // small delay
        SceneManager.LoadScene("OptionsUI");
    }

    private IEnumerator LoadLeaderboardWithSoundDelay()
    {
        yield return new WaitForSeconds(0.2f); // small delay
        SceneManager.LoadScene("LeaderboardUI");
    }

}
