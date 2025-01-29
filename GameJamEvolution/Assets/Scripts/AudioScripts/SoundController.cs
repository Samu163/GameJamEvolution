using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get; private set; }

    [Header("Sound Settings")]
    [SerializeField] private float masterVolume = 1f;
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Apply initial volume settings
        ApplyVolumeSettings();
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    private void ApplyVolumeSettings()
    {
        // Calculate final volumes
        float finalMusicVolume = musicVolume * masterVolume;
        float finalSFXVolume = sfxVolume * masterVolume;

        // Apply to music systems
        if (SoundTrackManager.Instance != null)
        {
            SoundTrackManager.Instance.SetMasterVolume(finalMusicVolume);
        }

        // Apply to SFX systems
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetMasterVolume(finalSFXVolume);
        }

        // Apply to Environment SFX
        if (EnvironmentSFXManager.Instance != null)
        {
            // We need to modify EnvironmentSFXManager to handle volume control
            // For now, it will continue playing at default volume
            EnvironmentSFXManager.Instance.PlayEnvironmentSound("AmbientSound");
        }

        // Log volume changes for debugging
        Debug.Log($"Applying volumes - Master: {masterVolume}, Music: {musicVolume}, SFX: {sfxVolume}");
        Debug.Log($"Final volumes - Music: {finalMusicVolume}, SFX: {finalSFXVolume}");

        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
        Debug.Log($"Master Volume set to: {masterVolume}");
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
        Debug.Log($"Music Volume set to: {musicVolume}");
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
        ApplyVolumeSettings();
        Debug.Log($"SFX Volume set to: {sfxVolume}");
    }

    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
} 