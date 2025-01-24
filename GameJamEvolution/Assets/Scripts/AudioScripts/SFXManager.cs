using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    
    [SerializeField] private List<SoundGroup> soundGroups = new List<SoundGroup>();
    private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();
    private const float MIN_TIME_BETWEEN_SOUNDS = 0.05f;
    
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
        foreach (var group in soundGroups)
        {
            foreach (var sound in group.sounds)
            {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;
                sound.source.loop = sound.loop;
                sound.source.playOnAwake = false;
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
    
    private void PlaySound(Sound sound, float volumeMultiplier)
    {
        if (sound == null || sound.source == null) return;
        
        // Check if sound is on cooldown
        if (soundCooldowns.ContainsKey(sound.name)) return;
        
        // Add cooldown
        soundCooldowns[sound.name] = sound.clip.length + MIN_TIME_BETWEEN_SOUNDS;
        
        // Play sound
        sound.source.volume = sound.volume * volumeMultiplier;
        sound.source.Play();
    }
    
    public void StopEffect(string groupName, string soundName)
    {
        var group = soundGroups.Find(g => g.groupName == groupName);
        if (group == null) return;
        
        var sound = group.GetSpecificSound(soundName);
        if (sound != null && sound.source != null)
        {
            sound.source.Stop();
            if (soundCooldowns.ContainsKey(sound.name))
            {
                soundCooldowns.Remove(sound.name);
            }
        }
    }
    
    public void StopAllEffects()
    {
        foreach (var group in soundGroups)
        {
            foreach (var sound in group.sounds)
            {
                if (sound.source != null)
                {
                    sound.source.Stop();
                }
            }
        }
        soundCooldowns.Clear();
    }
} 