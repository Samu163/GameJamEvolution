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
        public float fadeSpeed = 2f;
    }

    [Header("Music Configuration")]
    [SerializeField] private string trackName = "Track1";
    [SerializeField] private float crossFadeSpeed = 2f;
    
    [Header("Layer Settings")]
    [SerializeField] private List<LayerConfig> musicLayers = new List<LayerConfig>();

    private HashSet<int> activeLayerIndices = new HashSet<int>();
    public int deathCount = 0;

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
            // Start playing the base track
            SoundTrackManager.Instance.PlayMusic(trackName, false);
            
            // Initially disable ALL layers including layer 1
            foreach (var layer in musicLayers)
            {
                SoundTrackManager.Instance.SetLayerVolume(layer.layerIndex, 0f);
            }

            // Explicitly enable only layer 1
            var layer1Config = musicLayers.Find(l => l.layerIndex == 1);
            if (layer1Config != null)
            {
                EnableLayer(1);
                Debug.Log("Starting with Layer 1 only");
            }
        }
    }

    public void EnableLayer(int layerIndex)
    {
        if (!activeLayerIndices.Contains(layerIndex))
        {
            activeLayerIndices.Add(layerIndex);
            if (SoundTrackManager.Instance != null)
            {
                SoundTrackManager.Instance.StartLayer(layerIndex - 1); // Convert from 1-based to 0-based index
                StartFade(layerIndex, true);
            }
        }
    }

    public void DisableLayer(int layerIndex)
    {
        if (activeLayerIndices.Contains(layerIndex))
        {
            activeLayerIndices.Remove(layerIndex);
            StartFade(layerIndex, false);
        }
    }

    public void ToggleLayer(int layerIndex)
    {
        if (activeLayerIndices.Contains(layerIndex))
        {
            DisableLayer(layerIndex);
        }
        else
        {
            EnableLayer(layerIndex);
        }
    }

    private void StartFade(int layerIndex, bool fadeIn)
    {
        var layerConfig = musicLayers.Find(l => l.layerIndex == layerIndex);
        if (layerConfig == null || SoundTrackManager.Instance == null) return;

        float targetVolume = fadeIn ? layerConfig.targetVolume : 0f;
        SoundTrackManager.Instance.SetLayerVolume(layerIndex, targetVolume);
    }

    // Example methods for game events
    public void OnLevelComplete()
    {
        // Example: Enable layer 1 after completing first level
        if (LevelManager.Instance.levelCount == 1)
        {
            EnableLayer(1);
        }
        // Example: Enable layer 2 after level 3
        else if (LevelManager.Instance.levelCount == 3)
        {
            EnableLayer(2);
        }
    }

    public void OnPlayerDeath()
    {
        deathCount++;
        
        // Every two deaths, add a new layer
        if (deathCount % 2 == 0)
        {
            int layerToEnable = (deathCount / 2) + 1; // Start with layer 2
            if (layerToEnable <= 4) // Only up to layer 4
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
        
        // Reset all layers to 0 volume except layer 1
        foreach (var layer in musicLayers)
        {
            if (SoundTrackManager.Instance != null)
            {
                if (layer.layerIndex == 1)
                {
                    SoundTrackManager.Instance.SetLayerVolume(layer.layerIndex, layer.targetVolume);
                    activeLayerIndices.Add(1);
                }
                else
                {
                    SoundTrackManager.Instance.SetLayerVolume(layer.layerIndex, 0f);
                }
            }
        }
    }
} 