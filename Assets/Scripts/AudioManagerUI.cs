using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerUI : MonoBehaviour
{
    public static AudioManagerUI Instance;
    private AudioSource audioSource;

    public AudioClip clickSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps it across scenes
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
        else
        {
            Destroy(gameObject); // Only allow one instance
        }
    }

    public void PlayClick()
    {
        Debug.Log("PlayClick() called.");
        if (clickSound != null && audioSource != null)
        {
            Debug.Log("Playing sound...");
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogWarning("AudioSource or clickSound is missing!");
        }
    }
}
