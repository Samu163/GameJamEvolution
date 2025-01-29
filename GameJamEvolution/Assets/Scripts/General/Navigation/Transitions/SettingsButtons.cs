using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtons : MonoBehaviour
{
    public Button returnButton;
    public GameObject pauseCanvas;

    [Header("Audio Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    private SoundController soundController;

    private void Awake()
    {
        returnButton.onClick.AddListener(() => pauseCanvas.SetActive(true));
        returnButton.onClick.AddListener(() => gameObject.SetActive(false));

        // Initialize sound controller
        soundController = SoundController.Instance; // Use the singleton instance directly
        if (soundController == null)
        {
            Debug.LogWarning("SoundController not found!");
            return;
        }

        InitializeAudioSliders();
    }

    private void InitializeAudioSliders()
    {
        if (soundController != null)
        {
            // Remove any existing listeners
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();

            // Set initial values from SoundController
            musicVolumeSlider.value = soundController.GetMusicVolume();
            sfxVolumeSlider.value = soundController.GetSFXVolume();

            // Add listeners
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (soundController != null)
        {
            soundController.SetMusicVolume(value);
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (soundController != null)
        {
            soundController.SetSFXVolume(value);
        }
    }

    private void OnEnable()
    {
        // Update slider values when the settings panel becomes visible
        if (soundController != null)
        {
            musicVolumeSlider.value = soundController.GetMusicVolume();
            sfxVolumeSlider.value = soundController.GetSFXVolume();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
