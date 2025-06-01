using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionMenuUI : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadOptionsWithSoundDelay());
    }

    private IEnumerator LoadOptionsWithSoundDelay()
    {
        yield return new WaitForSeconds(0.2f); // small delay
        SceneManager.LoadScene("MainMenu");
    }

}
