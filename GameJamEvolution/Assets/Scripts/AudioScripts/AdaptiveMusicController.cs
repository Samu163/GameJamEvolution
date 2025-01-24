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
            SoundTrackManager.Instance.PlayMusic(trackName);
            // Initially disable all layers
            foreach (var layer in musicLayers)
            {
                SoundTrackManager.Instance.SetLayerVolume(layer.layerIndex, 0f);
            }
        }
    }

    public void EnableLayer(int layerIndex)
    {
        if (!activeLayerIndices.Contains(layerIndex))
        {
            activeLayerIndices.Add(layerIndex);
            StartFade(layerIndex, true);
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
        // Example: Toggle intensity based on death count
        // You can implement your own logic here
        int deathCount = 0; // Get this from your player or game manager
        
        if (deathCount >= 3)
        {
            EnableLayer(1);
        }
        if (deathCount >= 5)
        {
            EnableLayer(2);
        }
    }

    public void ResetLayers()
    {
        activeLayerIndices.Clear();
        foreach (var layer in musicLayers)
        {
            if (SoundTrackManager.Instance != null)
            {
                SoundTrackManager.Instance.SetLayerVolume(layer.layerIndex, 0f);
            }
        }
    }
} 