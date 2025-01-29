using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    
    [SerializeField] private List<SoundGroup> soundGroups = new List<SoundGroup>();
    private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();
    private const float MIN_TIME_BETWEEN_SOUNDS = 0.05f;
    
    [SerializeField] private float masterVolume = 1f;
    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    private const int AUDIO_SOURCE_POOL_SIZE = 10;  // Adjust this based on your needs
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Update()
    {
        // Update cooldowns
        var keys = soundCooldowns.Keys.ToList();
        foreach (var key in keys)
        {
            if (soundCooldowns[key] > 0)
            {
                soundCooldowns[key] -= Time.deltaTime;
                if (soundCooldowns[key] <= 0)
                {
                    soundCooldowns.Remove(key);
                }
            }
        }

    }
    
    private void InitializeAudioSources()
    {
        // Initialize audio source pool
        for (int i = 0; i < AUDIO_SOURCE_POOL_SIZE; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioSourcePool.Add(source);
        }

        // Initialize sound group sources
        foreach (var group in soundGroups)
        {
            foreach (var sound in group.sounds)
            {
                // We don't need to create individual sources for each sound anymore
                // Just validate the sound settings
                if (sound.clip == null)
                {
                    Debug.LogWarning($"Sound '{sound.name}' has no audio clip assigned!");
                }
            }
        }
    }
    
    private Sound FindSound(string soundName)
    {
        foreach (var group in soundGroups)
        {
            var sound = group.sounds.Find(s => s.name == soundName);
            if (sound != null)
            {
                return sound;
            }
        }
        Debug.LogWarning($"Sound '{soundName}' not found in any group");
        return null;
    }
    
    private AudioSource GetAvailableSource()
    {
        // First try to find an inactive source
        foreach (var source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // If all sources are playing, use the oldest one
        AudioSource oldestSource = audioSourcePool[0];
        float oldestTime = float.MaxValue;

        foreach (var source in audioSourcePool)
        {
            if (source.time < oldestTime)
            {
                oldestTime = source.time;
                oldestSource = source;
            }
        }

        return oldestSource;
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        
        // Update all playing sources
        foreach (var source in audioSourcePool)
        {
            if (source.isPlaying)
            {
                // Find the original sound to get its base volume
                foreach (var group in soundGroups)
                {
                    var sound = group.sounds.Find(s => s.clip == source.clip);
                    if (sound != null)
                    {
                        source.volume = sound.volume * masterVolume;
                        break;
                    }
                }
            }
        }
    }
    
    public void PlayEffect(string groupName, float volumeMultiplier = 1f)
    {
        var group = soundGroups.Find(g => g.groupName == groupName);
        if (group == null) return;
        
        var sound = group.GetNextSound();
        PlaySound(sound, volumeMultiplier);
    }
    
    public void PlaySpecificEffect(string groupName, string soundName, float volumeMultiplier = 1f)
    {
        var group = soundGroups.Find(g => g.groupName == groupName);
        if (group == null) return;
        
        var sound = group.GetSpecificSound(soundName);
        PlaySound(sound, volumeMultiplier);
    }
    
    public void PlaySoundWithVolume(string soundName, float volume)
    {
        if (string.IsNullOrEmpty(soundName)) return;
        
        Sound sound = FindSound(soundName);
        if (sound != null)
        {
            PlaySound(sound, volume);
        }
    }
    
    protected void PlaySound(Sound sound, float volumeMultiplier)
    {
        if (sound == null || sound.clip == null) return;

        AudioSource source = GetAvailableSource();
        if (source != null)
        {
            source.clip = sound.clip;
            source.volume = sound.volume * volumeMultiplier * masterVolume;
            source.loop = sound.loop;
            source.playOnAwake = false;
            source.Play();
        }
    }
    
    public void StopEffect(string groupName, string soundName)
    {
        var group = soundGroups.Find(g => g.groupName == groupName);
        if (group == null) return;
        
        var sound = group.GetSpecificSound(soundName);
        if (sound != null)
        {
            // Stop all pool sources playing this sound
            foreach (var source in audioSourcePool)
            {
                if (source.clip == sound.clip)
                {
                    source.Stop();
                }
            }
            
            if (soundCooldowns.ContainsKey(sound.name))
            {
                soundCooldowns.Remove(sound.name);
            }
        }
    }
    
    public void StopAllEffects()
    {
        foreach (var source in audioSourcePool)
        {
            source.Stop();
        }
        soundCooldowns.Clear();
    }

    // Add these methods for easy access to player sounds
    public void PlayPlayerSound(string soundName, float volumeMultiplier = 1f)
    {
        PlaySpecificEffect("Player", soundName, volumeMultiplier);
    }

    public void PlayPlayerMovementSound(float volumeMultiplier = 1f)
    {
        PlayEffect("PlayerMovement", volumeMultiplier);
    }

    public void PlayPlayerJumpSound(float volumeMultiplier = 1f)
    {
        PlayEffect("PlayerJump", volumeMultiplier);
    }

    public void PlayPlayerLandSound(float volumeMultiplier = 1f)
    {
        PlayEffect("PlayerLand", volumeMultiplier);
    }

    public void PlayPlayerDeathSound(float volumeMultiplier = 1f)
    {
        PlayEffect("PlayerDeath", volumeMultiplier);
    }
} 