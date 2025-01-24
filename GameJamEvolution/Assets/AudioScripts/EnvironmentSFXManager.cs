using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnvironmentSFXManager : MonoBehaviour
{
    public static EnvironmentSFXManager Instance { get; private set; }

    [SerializeField] private List<EnvironmentSoundGroup> soundGroups = new List<EnvironmentSoundGroup>();
    [SerializeField] private float crossFadeDuration = 0.5f; // Duration of crossfade in seconds
    
    private Dictionary<string, EnvironmentSoundGroup> groupDictionary = new Dictionary<string, EnvironmentSoundGroup>();
    private Dictionary<EnvironmentSoundGroup, float> nextGroupPlayTime = new Dictionary<EnvironmentSoundGroup, float>();
    private Dictionary<EnvironmentSoundGroup, Coroutine> fadeCoroutines = new Dictionary<EnvironmentSoundGroup, Coroutine>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            InitializeGroups();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGroups()
    {
        groupDictionary.Clear();
        nextGroupPlayTime.Clear();
        fadeCoroutines.Clear();
        foreach (var group in soundGroups)
        {
            if (group != null && !string.IsNullOrEmpty(group.groupName))
            {
                groupDictionary[group.groupName] = group;
                nextGroupPlayTime[group] = 0f;
            }
        }
    }
    
    private void InitializeAudioSources()
    {
        foreach (var group in soundGroups)
        {
            if (group == null) continue;
            
            foreach (var soundEntry in group.sounds)
            {
                soundEntry.source = gameObject.AddComponent<AudioSource>();
                soundEntry.source.clip = soundEntry.clip;
                soundEntry.source.volume = 0f;
                soundEntry.source.loop = false;
                soundEntry.source.playOnAwake = false;
            }
        }
    }

    private void Update()
    {
        // Only check for continuous playback for looping groups
        foreach (var currentGroup in soundGroups)
        {
            if (currentGroup == null || !currentGroup.isLoopingGroup) continue;

            // Check if any sound in the group is currently playing
            bool isAnyPlaying = false;
            foreach (var groupSound in currentGroup.sounds)
            {
                if (groupSound.isPlaying && groupSound.source.isPlaying)
                {
                    isAnyPlaying = true;
                    break;
                }
            }

            // If no sound is playing and it's time for the next sound
            if (!isAnyPlaying && Time.time >= nextGroupPlayTime[currentGroup])
            {
                PlayEnvironmentSound(currentGroup);
            }
        }
    }

    public void PlayEnvironmentSound(EnvironmentSoundGroup group)
    {
        if (group == null) return;

        if (group.isLoopingGroup)
        {
            // For looping groups, stop other sounds in the group
            foreach (var groupSound in group.sounds)
            {
                if (groupSound.isPlaying)
                {
                    StopSound(groupSound);
                }
            }
        }

        var nextSound = group.GetNextSound();
        if (nextSound != null)
        {
            PlaySound(nextSound, group);
        }
    }

    public void PlayEnvironmentSound(string groupName)
    {
        if (groupDictionary.TryGetValue(groupName, out var group))
        {
            PlayEnvironmentSound(group);
        }
    }

    public void PlayEnvironmentSound(EnvironmentSoundGroup group, string soundName)
    {
        if (group == null) return;
        var foundSound = group.GetSpecificSound(soundName);
        if (foundSound != null)
        {
            PlaySound(foundSound, group);
        }
    }

    private void PlaySound(EnvironmentSoundGroup.EnvironmentSound soundToPlay, EnvironmentSoundGroup group)
    {
        if (soundToPlay == null || soundToPlay.source == null) return;

        if (group.isLoopingGroup)
        {
            // For looping sounds, use crossfade
            if (fadeCoroutines.ContainsKey(group) && fadeCoroutines[group] != null)
            {
                StopCoroutine(fadeCoroutines[group]);
            }
            fadeCoroutines[group] = StartCoroutine(CrossFadeSound(soundToPlay, group));
        }
        else
        {
            // For one-shot sounds, play directly
            soundToPlay.isPlaying = true;
            soundToPlay.source.volume = soundToPlay.volume;
            soundToPlay.source.Play();
        }

        // Schedule next play time based on clip length and interval
        float clipLength = soundToPlay.clip != null ? soundToPlay.clip.length : 0f;
        
        if (group.isLoopingGroup)
        {
            // For looping sounds, schedule next play considering crossfade
            if (soundToPlay.playInterval == 0)
            {
                nextGroupPlayTime[group] = Time.time + clipLength - crossFadeDuration;
            }
            else
            {
                nextGroupPlayTime[group] = Time.time + clipLength + soundToPlay.playInterval;
            }
        }
    }

    private void StopSound(EnvironmentSoundGroup.EnvironmentSound soundToStop)
    {
        if (soundToStop == null || soundToStop.source == null) return;
        
        soundToStop.isPlaying = false;
        StartCoroutine(FadeOutSound(soundToStop));
    }

    private IEnumerator FadeOutSound(EnvironmentSoundGroup.EnvironmentSound sound)
    {
        float startVolume = sound.source.volume;
        float elapsed = 0f;

        while (elapsed < crossFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / crossFadeDuration;
            // Use exponential curve for volume fading
            float fadeValue = Mathf.Pow(1f - t, 2f);
            sound.source.volume = startVolume * fadeValue;
            yield return null;
        }

        sound.source.Stop();
        sound.source.volume = 0f;
    }

    private IEnumerator CrossFadeSound(EnvironmentSoundGroup.EnvironmentSound newSound, EnvironmentSoundGroup group)
    {
        // Get currently playing sounds before starting the new one
        List<(EnvironmentSoundGroup.EnvironmentSound sound, float startVolume)> fadingOutSounds = new List<(EnvironmentSoundGroup.EnvironmentSound, float)>();
        
        foreach (var groupSound in group.sounds)
        {
            if (groupSound != newSound && groupSound.isPlaying && groupSound.source.isPlaying)
            {
                fadingOutSounds.Add((groupSound, groupSound.source.volume));
                groupSound.isPlaying = false; // Mark it as not playing but don't stop it yet
            }
        }

        // Start playing the new sound at 0 volume
        newSound.isPlaying = true;
        newSound.source.volume = 0f;
        newSound.source.Play();

        float elapsed = 0f;
        while (elapsed < crossFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / crossFadeDuration;

            // Use exponential curves for more natural volume transitions
            float fadeInValue = Mathf.Pow(t, 2f); // Quadratic fade in
            float fadeOutValue = Mathf.Pow(1f - t, 2f); // Quadratic fade out

            // Fade in new sound
            newSound.source.volume = newSound.volume * fadeInValue;

            // Fade out old sounds
            foreach (var (oldSound, startVolume) in fadingOutSounds)
            {
                if (oldSound.source != null)
                {
                    oldSound.source.volume = startVolume * fadeOutValue;
                }
            }

            yield return null;
        }

        // Ensure final volumes are set and stop old sounds
        newSound.source.volume = newSound.volume;
        foreach (var (oldSound, _) in fadingOutSounds)
        {
            if (oldSound.source != null)
            {
                oldSound.source.Stop();
                oldSound.source.volume = 0f;
            }
        }

        fadeCoroutines[group] = null;
    }

    private void PlaySoundDirectly(EnvironmentSoundGroup.EnvironmentSound soundToPlay)
    {
        if (soundToPlay == null || soundToPlay.source == null) return;
        soundToPlay.source.Play();
    }

    public void StopEnvironmentSound(EnvironmentSoundGroup group, string soundName)
    {
        if (group == null) return;
        var foundSound = group.GetSpecificSound(soundName);
        if (foundSound != null)
        {
            StopSound(foundSound);
        }
    }

    public void StopEnvironmentSound(string groupName, string soundName)
    {
        if (this == null) return; // Check if manager is being destroyed
        if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(soundName)) return;
        
        if (groupDictionary.TryGetValue(groupName, out var group))
        {
            var sound = group.GetSpecificSound(soundName);
            if (sound != null && sound.source != null)
            {
                if (group.isLoopingGroup)
                {
                    StopSound(sound); // Fade out looping sounds
                }
                else
                {
                    // For one-shot sounds, stop immediately
                    sound.isPlaying = false;
                    if (sound.source != null)
                    {
                        sound.source.Stop();
                        sound.source.volume = 0f;
                    }
                }
            }
        }
    }

    public void StopAllSounds()
    {
        foreach (var group in soundGroups)
        {
            if (group == null) continue;
            
            // Cancel any ongoing fades for this group
            if (fadeCoroutines.ContainsKey(group) && fadeCoroutines[group] != null)
            {
                StopCoroutine(fadeCoroutines[group]);
                fadeCoroutines[group] = null;
            }

            foreach (var groupSound in group.sounds)
            {
                if (groupSound.isPlaying)
                {
                    StopSound(groupSound);
                }
            }
        }
    }

    private void OnDisable()
    {
        // Immediately stop all sounds without fading when disabled
        foreach (var group in soundGroups)
        {
            if (group == null) continue;
            
            foreach (var groupSound in group.sounds)
            {
                if (groupSound.isPlaying && groupSound.source != null)
                {
                    groupSound.isPlaying = false;
                    groupSound.source.Stop();
                    groupSound.source.volume = 0f;
                }
            }
        }
    }
} 