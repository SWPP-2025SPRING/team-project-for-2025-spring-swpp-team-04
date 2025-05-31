using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadOptionsWithSoundDelay());
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game from Pause Menu.");
    }

    public void Restart()
    {
        Time.timeScale = 1f; // Make sure time resumes
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator LoadOptionsWithSoundDelay()
    {
        yield return new WaitForSeconds(0.2f); // small delay
        SceneManager.LoadScene("MainMenu");
    }
}
