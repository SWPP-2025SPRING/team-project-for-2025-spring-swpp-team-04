using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    public GameObject mainMenuUI;


    public void StartGame()
    {
        SceneManager.LoadScene("Track_2"); // 👈 your racing scene name
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }
}
