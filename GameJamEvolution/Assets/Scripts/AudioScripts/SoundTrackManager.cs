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
    [SerializeField] private float scheduleAheadTime = 0.1f;

    private Dictionary<string, List<AudioSource>> trackSources = new Dictionary<string, List<AudioSource>>();
    private Dictionary<string, Dictionary<int, Coroutine>> trackFadeCoroutines = new Dictionary<string, Dictionary<int, Coroutine>>();
    private string currentTrackName;
    private double nextStartTime;
    private bool isPlaying = false;

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
                source.loop = false;
                source.playOnAwake = false;
                source.volume = 0f;
                source.priority = 0;
                sources.Add(source);
            }
        }
    }

    private void Update()
    {
        if (!isPlaying || string.IsNullOrEmpty(currentTrackName)) return;

        double currentTime = AudioSettings.dspTime;
        if (currentTime + scheduleAheadTime > nextStartTime)
        {
            ScheduleNextLoop();
        }
    }

    private void ScheduleNextLoop()
    {
        if (!trackSources.ContainsKey(currentTrackName)) return;

        var sources = trackSources[currentTrackName];
        if (sources.Count == 0 || sources[0].clip == null) return;

        double currentTime = AudioSettings.dspTime;
        
        if (nextStartTime <= currentTime)
        {
            nextStartTime = currentTime + scheduleAheadTime;
        }

        foreach (var source in sources)
        {
            source.PlayScheduled(nextStartTime);
        }

        nextStartTime += sources[0].clip.length;
    }

    public void PlayTrack(string trackName)
    {
        if (!trackSources.ContainsKey(trackName)) return;

        if (currentTrackName != null && currentTrackName != trackName)
        {
            StopTrack(currentTrackName);
        }

        currentTrackName = trackName;
        isPlaying = true;
        nextStartTime = AudioSettings.dspTime;
        ScheduleNextLoop();
    }

    public void StopTrack(string trackName)
    {
        if (!trackSources.ContainsKey(trackName)) return;

        isPlaying = false;
        foreach (var source in trackSources[trackName])
        {
            source.Stop();
        }

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
        
        if (fadeCoroutines.ContainsKey(layerIndex))
        {
            if (fadeCoroutines[layerIndex] != null)
                StopCoroutine(fadeCoroutines[layerIndex]);
            fadeCoroutines.Remove(layerIndex);
        }

        fadeCoroutines[layerIndex] = StartCoroutine(
            FadeLayerCoroutine(trackName, layerIndex, targetVolume, fadeTime < 0 ? defaultFadeTime : fadeTime));
    }

    private IEnumerator FadeLayerCoroutine(string trackName, int layerIndex, float targetVolume, float fadeTime)
    {
        AudioSource source = trackSources[trackName][layerIndex];
        float startVolume = source.volume;
        float elapsed = 0;

        // Get the layer's base volume
        var track = musicTracks.Find(t => t.trackName == trackName);
        if (track == null || layerIndex >= track.layers.Count) yield break;
        
        float layerBaseVolume = track.layers[layerIndex].volume;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeTime;
            // Calculate the target volume considering both the layer's base volume and master volume
            float currentTargetVolume = targetVolume * layerBaseVolume * masterVolume;
            source.volume = Mathf.Lerp(startVolume, currentTargetVolume, t);
            yield return null;
        }

        source.volume = targetVolume * layerBaseVolume * masterVolume;
        trackFadeCoroutines[trackName].Remove(layerIndex);
    }

    public void SetMasterVolume(float volume)
    {
        Debug.Log($"SoundTrackManager - Setting master volume to: {volume}");
        masterVolume = Mathf.Clamp01(volume);
        
        if (string.IsNullOrEmpty(currentTrackName)) return;
        
        // Get the current track
        var currentTrack = musicTracks.Find(t => t.trackName == currentTrackName);
        if (currentTrack == null) return;

        // Update all active sources with their proper layer volume * master volume
        var sources = trackSources[currentTrackName];
        for (int i = 0; i < sources.Count; i++)
        {
            if (i < currentTrack.layers.Count)
            {
                float layerVolume = currentTrack.layers[i].volume;
                sources[i].volume = layerVolume * masterVolume;
                Debug.Log($"Layer {i} volume set to: {sources[i].volume} (layer: {layerVolume} * master: {masterVolume})");
            }
        }
    }

    public string GetCurrentTrackName()
    {
        return currentTrackName;
    }
} 