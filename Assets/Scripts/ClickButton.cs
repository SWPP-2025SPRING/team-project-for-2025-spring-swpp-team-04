using UnityEngine;
using UnityEngine.UI;

public class ClickButton: MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        Button button = GetComponent<Button>();
        if (button != null && clickSound != null)
        {
            Debug.Log(gameObject.name + " clicked, playing sound");
            button.onClick.AddListener(() => audioSource.PlayOneShot(clickSound));
        }

        else
        {
            Debug.LogWarning("Button or clickSound missing on " + gameObject.name);
        }
    }
}
