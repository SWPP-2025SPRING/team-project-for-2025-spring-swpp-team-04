using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelector : MonoBehaviour
{

    public GameObject levelSelectorUI;
    public AudioSource selectLevelMusic;

    void Start()
    {
        // Load and apply saved music volume
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (selectLevelMusic != null)
        {
            selectLevelMusic.volume = savedVolume;
        }
        else
        {
            Debug.LogWarning("SelectLevelMusic AudioSource not assigned.");
        }
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Track_2");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("S1");
    }

    public void BackToMainMenu()
    {
        StartCoroutine(LoadOptionsWithSoundDelay());
    }

    private IEnumerator LoadOptionsWithSoundDelay()
    {
        yield return new WaitForSeconds(0.2f); // small delay
        SceneManager.LoadScene("MainMenu");
    }
}
