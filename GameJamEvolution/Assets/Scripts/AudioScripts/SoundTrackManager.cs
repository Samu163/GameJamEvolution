using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrackManager : MonoBehaviour
{
    public static SoundTrackManager Instance { get; private set; }

    [SerializeField] private SoundTrack[] musicTracks;
    [SerializeField] private float masterVolume = 1f;

    private Dictionary<string, SoundTrack> trackDictionary = new Dictionary<string, SoundTrack>();
    private AudioSource mainSource;
    private AudioSource[] layerSources;
    private string currentTrackName;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
            InitializeTrackDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        mainSource = gameObject.AddComponent<AudioSource>();
        mainSource.playOnAwake = false;
        mainSource.loop = true;
        mainSource.priority = 0;
        
        // Initialize layer sources (for adaptive music)
        layerSources = new AudioSource[4]; // Support up to 4 layers
        for (int i = 0; i < layerSources.Length; i++)
        {
            layerSources[i] = gameObject.AddComponent<AudioSource>();
            layerSources[i].playOnAwake = false;
            layerSources[i].loop = true;
            layerSources[i].priority = 0;
        }
    }

    private void InitializeTrackDictionary()
    {
        foreach (var track in musicTracks)
        {
            if (!string.IsNullOrEmpty(track.name) && track.clip != null)
            {
                trackDictionary[track.name] = track;
            }
        }
    }

    public void PlayMusic(string trackName, bool fadeIn = true)
    {
        if (!trackDictionary.ContainsKey(trackName)) return;
        if (currentTrackName == trackName) return;

        var track = trackDictionary[trackName];

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // If we're already playing something, fade it out first
        if (mainSource.isPlaying && fadeIn)
        {
            fadeCoroutine = StartCoroutine(CrossFade(track));
        }
        else
        {
            PlayTrackDirectly(track, fadeIn);
        }

        currentTrackName = trackName;
    }

    private void PlayTrackDirectly(SoundTrack track, bool fadeIn)
    {
        // Setup main source (base layer)
        mainSource.clip = track.clip;
        mainSource.loop = track.loop;
        mainSource.volume = fadeIn ? 0f : track.volume * masterVolume;
        
        double startTime = AudioSettings.dspTime + 0.1;
        mainSource.PlayScheduled(startTime);

        if (fadeIn)
        {
            fadeCoroutine = StartCoroutine(FadeIn(track.fadeInDuration, track.volume));
        }

        // Setup layers if they exist
        if (track.layers != null && track.layers.Length > 0)
        {
            for (int i = 0; i < track.layers.Length && i < layerSources.Length; i++)
            {
                layerSources[i].clip = track.layers[i];
                layerSources[i].loop = track.loop;
                layerSources[i].volume = 0f; // Start all layers muted
                
                // Only schedule the base layer (index 0) to play initially
                if (i == 0)
                {
                    layerSources[i].PlayScheduled(startTime);
                    layerSources[i].volume = track.layerVolumes != null && track.layerVolumes.Length > i 
                        ? track.layerVolumes[i] * masterVolume 
                        : track.volume * masterVolume;
                }
            }
        }
    }

    private IEnumerator CrossFade(SoundTrack newTrack)
    {
        float fadeOutDuration = trackDictionary[currentTrackName].fadeOutDuration;
        float fadeInDuration = newTrack.fadeInDuration;
        float elapsed = 0f;

        // Store the initial volumes
        float startVolume = mainSource.volume;
        AudioSource nextSource = gameObject.AddComponent<AudioSource>();
        nextSource.clip = newTrack.clip;
        nextSource.loop = newTrack.loop;
        nextSource.volume = 0f;
        nextSource.Play();

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            mainSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
            nextSource.volume = Mathf.Lerp(0f, newTrack.volume * masterVolume, elapsed / fadeInDuration);
            yield return null;
        }

        mainSource.Stop();
        mainSource.clip = newTrack.clip;
        mainSource.loop = newTrack.loop;
        mainSource.volume = newTrack.volume * masterVolume;
        mainSource.Play();

        Destroy(nextSource);
    }

    private IEnumerator FadeIn(float duration, float targetVolume)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mainSource.volume = Mathf.Lerp(0f, targetVolume * masterVolume, elapsed / duration);
            yield return null;
        }
    }

    public void SetLayerVolume(int layerIndex, float volume)
    {
        if (layerIndex >= 0 && layerIndex < layerSources.Length)
        {
            layerSources[layerIndex].volume = Mathf.Clamp01(volume) * masterVolume;
        }
    }

    public void StopMusic(bool fadeOut = true)
    {
        if (fadeOut)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeOut(trackDictionary[currentTrackName].fadeOutDuration));
        }
        else
        {
            mainSource.Stop();
            foreach (var source in layerSources)
            {
                source.Stop();
            }
        }
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVolume = mainSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            mainSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        mainSource.Stop();
        foreach (var source in layerSources)
        {
            source.Stop();
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (mainSource.isPlaying)
        {
            mainSource.volume = trackDictionary[currentTrackName].volume * masterVolume;
        }
    }

    // Add this method to start playing a specific layer
    public void StartLayer(int layerIndex)
    {
        if (layerIndex >= 0 && layerIndex < layerSources.Length && layerSources[layerIndex] != null)
        {
            if (!layerSources[layerIndex].isPlaying)
            {
                double startTime = AudioSettings.dspTime + 0.1;
                layerSources[layerIndex].PlayScheduled(startTime);
            }
        }
    }
} 