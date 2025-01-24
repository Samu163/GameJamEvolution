using UnityEngine;

public class SoundController : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private float masterVolume = 1f;
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;

    private void Start()
    {
        // Initialize volumes
        if (SoundTrackManager.Instance != null)
        {
            SoundTrackManager.Instance.SetMasterVolume(musicVolume);
        }
        
        if (SFXManager.Instance != null)
        {
            // Assuming SFXManager has volume control
            //SFXManager.Instance.SetVolume(sfxVolume);
        }

        if (EnvironmentSFXManager.Instance != null)
        {
            SoundTrackManager.Instance.PlayTrack("Track1");
            EnvironmentSFXManager.Instance.PlayEnvironmentSound("AmbientSound");

        }
    }

    // Volume Control Methods
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (SoundTrackManager.Instance != null)
        {
            SoundTrackManager.Instance.SetMasterVolume(musicVolume * masterVolume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (SFXManager.Instance != null)
        {
            // Assuming SFXManager has volume control
            // SFXManager.Instance.SetVolume(sfxVolume * masterVolume);
        }
    }
} 