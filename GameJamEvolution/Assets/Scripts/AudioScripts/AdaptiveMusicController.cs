using UnityEngine;
using System.Collections.Generic;

public class AdaptiveMusicController : MonoBehaviour
{
    public static AdaptiveMusicController Instance { get; private set; }

    [System.Serializable]
    public class LayerConfig
    {
        public int layerIndex;
        public float targetVolume = 1f;
        public float fadeTime = 1f;
    }

    [Header("Music Configuration")]
    [SerializeField] private string currentTrackName = "Track1";
    
    [Header("Layer Settings")]
    [SerializeField] private List<LayerConfig> musicLayers = new List<LayerConfig>();
    
    private HashSet<int> activeLayerIndices = new HashSet<int>();
    public int deathCount { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SoundTrackManager.Instance != null)
        {
            SoundTrackManager.Instance.PlayTrack(currentTrackName);
            // Start with all layers faded out
            foreach (var layer in musicLayers)
            {
                SoundTrackManager.Instance.FadeTrackLayer(currentTrackName, layer.layerIndex, 0f, 0f);
            }
            // Enable first layer
            EnableLayer(0);
        }
    }

    public void ChangeTrack(string newTrackName)
    {
        if (SoundTrackManager.Instance != null && currentTrackName != newTrackName)
        {
            currentTrackName = newTrackName;
            SoundTrackManager.Instance.PlayTrack(newTrackName);
            
            // Reset layers for new track
            foreach (var layer in musicLayers)
            {
                SoundTrackManager.Instance.FadeTrackLayer(currentTrackName, layer.layerIndex, 0f, 0f);
            }
            EnableLayer(0);
        }
    }

    public void EnableLayer(int layerIndex)
    {
        var layerConfig = musicLayers.Find(l => l.layerIndex == layerIndex);
        if (layerConfig != null && SoundTrackManager.Instance != null)
        {
            activeLayerIndices.Add(layerIndex);
            SoundTrackManager.Instance.FadeTrackLayer(currentTrackName, layerIndex, 
                layerConfig.targetVolume, layerConfig.fadeTime);
        }
    }

    public void DisableLayer(int layerIndex)
    {
        var layerConfig = musicLayers.Find(l => l.layerIndex == layerIndex);
        if (layerConfig != null && SoundTrackManager.Instance != null)
        {
            activeLayerIndices.Remove(layerIndex);
            SoundTrackManager.Instance.FadeTrackLayer(currentTrackName, layerIndex, 0f, layerConfig.fadeTime);
        }
    }

    public void FadeLayer(int layerIndex, float targetVolume, float fadeTime)
    {
        if (SoundTrackManager.Instance != null)
        {
            SoundTrackManager.Instance.FadeTrackLayer(currentTrackName, layerIndex, targetVolume, fadeTime);
        }
    }

    public void OnPlayerDeath()
    {
        deathCount++;
        if (deathCount % 2 == 0)
        {
            int layerToEnable = deathCount / 2;
            if (layerToEnable < musicLayers.Count)
            {
                EnableLayer(layerToEnable);
                Debug.Log($"Death count: {deathCount}. Enabling layer {layerToEnable}");
            }
        }
    }

    public void ResetLayers()
    {
        deathCount = 0;
        activeLayerIndices.Clear();
        
        foreach (var layer in musicLayers)
        {
            if (layer.layerIndex == 0)
            {
                EnableLayer(0);
            }
            else
            {
                DisableLayer(layer.layerIndex);
            }
        }
    }

    // Example of a custom layer control method
    public void SetIntensityByHeight(float height)
    {
        float normalizedHeight = Mathf.Clamp01(height / 20f); // Assuming 20 units is max height
        SetLayerVolume(1, normalizedHeight * 0.7f); // Layer 2 fades in with height
        SetLayerVolume(2, Mathf.Max(0, normalizedHeight - 0.5f) * 2 * 0.5f); // Layer 3 starts at half height
    }

    public void SetLayerVolume(int layerIndex, float volume)
    {
        if (SoundTrackManager.Instance != null)
        {
            SoundTrackManager.Instance.FadeTrackLayer(currentTrackName, layerIndex, volume, 0f); // Instant volume change
        }
    }
} 