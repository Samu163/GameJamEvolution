using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrackManager : MonoBehaviour
{
    public static SoundTrackManager Instance { get; private set; }

    [System.Serializable]
    public class MusicLayer
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
    }

    [System.Serializable]
    public class MusicTrack
    {
        public string trackName;
        public List<MusicLayer> layers = new List<MusicLayer>();
    }

    [SerializeField] private List<MusicTrack> musicTracks = new List<MusicTrack>();
    [SerializeField] private float masterVolume = 1f;
    [SerializeField] private float defaultFadeTime = 1f;

    private Dictionary<string, List<AudioSource>> trackSources = new Dictionary<string, List<AudioSource>>();
    private Dictionary<string, Dictionary<int, Coroutine>> trackFadeCoroutines = new Dictionary<string, Dictionary<int, Coroutine>>();
    private string currentTrackName;

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

    private void InitializeAudioSources()
    {
        foreach (var track in musicTracks)
        {
            List<AudioSource> sources = new List<AudioSource>();
            trackSources[track.trackName] = sources;
            trackFadeCoroutines[track.trackName] = new Dictionary<int, Coroutine>();

            for (int i = 0; i < track.layers.Count; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = track.layers[i].clip;
                source.loop = true;
                source.playOnAwake = false;
                source.volume = 0f;
                source.priority = 0;
                sources.Add(source);
            }
        }
    }

    public void PlayTrack(string trackName)
    {
        if (!trackSources.ContainsKey(trackName)) return;

        // Stop current track if different
        if (currentTrackName != null && currentTrackName != trackName)
        {
            StopTrack(currentTrackName);
        }

        currentTrackName = trackName;
        double startTime = AudioSettings.dspTime + 0.1;
        foreach (var source in trackSources[trackName])
        {
            source.PlayScheduled(startTime);
        }
    }

    public void StopTrack(string trackName)
    {
        if (!trackSources.ContainsKey(trackName)) return;

        foreach (var source in trackSources[trackName])
        {
            source.Stop();
        }

        // Clear any active fades
        if (trackFadeCoroutines.ContainsKey(trackName))
        {
            foreach (var coroutine in trackFadeCoroutines[trackName].Values)
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);
            }
            trackFadeCoroutines[trackName].Clear();
        }
    }

    public void FadeTrackLayer(string trackName, int layerIndex, float targetVolume, float fadeTime = -1)
    {
        if (!trackSources.ContainsKey(trackName)) return;
        if (layerIndex < 0 || layerIndex >= trackSources[trackName].Count) return;

        var fadeCoroutines = trackFadeCoroutines[trackName];
        
        // Cancel existing fade for this layer
        if (fadeCoroutines.ContainsKey(layerIndex))
        {
            if (fadeCoroutines[layerIndex] != null)
                StopCoroutine(fadeCoroutines[layerIndex]);
            fadeCoroutines.Remove(layerIndex);
        }

        // Start new fade
        fadeCoroutines[layerIndex] = StartCoroutine(
            FadeLayerCoroutine(trackName, layerIndex, targetVolume, fadeTime < 0 ? defaultFadeTime : fadeTime));
    }

    private IEnumerator FadeLayerCoroutine(string trackName, int layerIndex, float targetVolume, float fadeTime)
    {
        AudioSource source = trackSources[trackName][layerIndex];
        float startVolume = source.volume;
        float elapsed = 0;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume * masterVolume, t);
            yield return null;
        }

        source.volume = targetVolume * masterVolume;
        trackFadeCoroutines[trackName].Remove(layerIndex);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        foreach (var sources in trackSources.Values)
        {
            foreach (var source in sources)
            {
                float currentLayerVolume = source.volume / masterVolume;
                source.volume = currentLayerVolume * masterVolume;
            }
        }
    }

    public string GetCurrentTrackName()
    {
        return currentTrackName;
    }
} 