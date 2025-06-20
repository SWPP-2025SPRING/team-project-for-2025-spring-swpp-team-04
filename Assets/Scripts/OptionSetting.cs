using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class OptionSetting : MonoBehaviour
{
    [Header("Resolution")]
    public Dropdown resolutionDropdown;
    Resolution[] resolutions;

    [Header("Brightness")]
    public Light mainLight;
    public Slider brightnessSlider;

    [Header("Music Volume")]
    public AudioSource mainMenuMusic;
    public Slider volumeSlider;

    [Header("Car Selection")]
    public Dropdown carDropdown;
    public string[] carNames;

    void Start()
    {
        SetupResolutionDropdown();
        SetupCarDropdown();
        SetupBrightnessSlider();
        SetupVolumeSlider();
    }

    // --- Resolution Setup ---
    void SetupResolutionDropdown()
    {
      resolutions = Screen.resolutions;
      resolutionDropdown.ClearOptions();

      List<string> options = new List<string>();
      options.Add("Resolution"); // Placeholder

      int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0); // default to placeholder

      for (int i = 0; i < resolutions.Length; i++)
      {
          string option = resolutions[i].width + " x " + resolutions[i].height;
          options.Add(option);
      }

      resolutionDropdown.AddOptions(options);

      // Set dropdown to saved value (0 = placeholder)
      resolutionDropdown.value = savedResolutionIndex;
      resolutionDropdown.RefreshShownValue();

      resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int index)
    {
        if (index == 0)
        {
            Debug.Log("No resolution selected yet.");
            return;
        }

        Resolution resolution = resolutions[index - 1];
        Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.FullScreenWindow);

        // Save selected resolution
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();

        Debug.Log("Resolution changed to: " + resolution.width + "x" + resolution.height);
    }

    // --- Car Setup ---
    void SetupCarDropdown()
    {
        carDropdown.ClearOptions();
        List<string> options = new List<string>(carNames);
        carDropdown.AddOptions(options);

        // 이전에 선택한 값 불러오기
        int savedCarIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0);
        carDropdown.value = savedCarIndex;
        carDropdown.RefreshShownValue();

        carDropdown.onValueChanged.AddListener(SetCar);
    }

    public void SetCar(int index)
    {
        PlayerPrefs.SetInt("SelectedCarIndex", index);
        PlayerPrefs.Save();
        Debug.Log($"Car Selected: {carNames[index]}");
    }



    // --- Brightness Setup ---
    void SetupBrightnessSlider()
    {
        if (mainLight != null && brightnessSlider != null)
        {
            // Load saved brightness or default to 1
            float savedBrightness = PlayerPrefs.GetFloat("Brightness", 1f);

            brightnessSlider.value = savedBrightness;
            mainLight.intensity = savedBrightness;

            brightnessSlider.onValueChanged.AddListener(AdjustBrightness);
        }
    }

    public void AdjustBrightness(float value)
    {
        if (mainLight != null)
        {
            mainLight.intensity = value;

            // Save the brightness value
            PlayerPrefs.SetFloat("Brightness", value);
            PlayerPrefs.Save();

            Debug.Log("Light intensity set to: " + mainLight.intensity);
        }
        else
        {
            Debug.LogWarning("Main Light is not assigned!");
        }
    }

    // --- Music Volume Setup ---
    void SetupVolumeSlider()
    {
        if (mainMenuMusic != null && volumeSlider != null)
        {
            // Load saved volume or use current volume if not set
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", mainMenuMusic.volume);
            volumeSlider.value = savedVolume;
            mainMenuMusic.volume = savedVolume;

            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void SetVolume(float value)
    {
        if (mainMenuMusic != null)
        {
            mainMenuMusic.volume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);  // Save volume
            PlayerPrefs.Save();                          // Ensure it's written to disk
            Debug.Log("Music volume set to: " + value);
        }
        else
        {
            Debug.LogWarning("Main Menu Music AudioSource is not assigned!");
        }
    }
}
