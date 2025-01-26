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

    [System.Serializable]
    public class LevelLayerRule
    {
        public int minLevel;
        public int maxLevel;
        [Tooltip("Indices of layers to enable in this level range")]
        public int[] layersToEnable;
    }

    [Header("Music Configuration")]
    [SerializeField] private string currentTrackName = "Track1";
    
    [Header("Layer Settings")]
    [SerializeField] private List<LayerConfig> musicLayers = new List<LayerConfig>();
    
    [Header("Level Rules")]
    [SerializeField] private List<LevelLayerRule> levelRules = new List<LevelLayerRule>();

    private HashSet<int> activeLayerIndices = new HashSet<int>();
    
    [Header("Debug Info")]
    [SerializeField, ReadOnly] private int _deathCount = 0;
    [SerializeField, ReadOnly] private int _currentLevel = 0;
    
    public int deathCount 
    { 
        get => _deathCount;
        private set 
        {
            _deathCount = value;
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
    }

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
            UpdateLayersForCurrentLevel();
        }
    }

    private void Update()
    {
        // Check for level changes
        if (LevelManager.Instance != null && _currentLevel != LevelManager.Instance.levelCount)
        {
            _currentLevel = LevelManager.Instance.levelCount;
            UpdateLayersForCurrentLevel();
        }
    }

    private void UpdateLayersForCurrentLevel()
    {
        // First, disable all layers
        foreach (var layer in musicLayers)
        {
            DisableLayer(layer.layerIndex);
        }

        // Find and apply the matching rule for current level
        foreach (var rule in levelRules)
        {
            if (_currentLevel >= rule.minLevel && _currentLevel <= rule.maxLevel)
            {
                foreach (int layerIndex in rule.layersToEnable)
                {
                    EnableLayer(layerIndex);
                }
                break; // Use first matching rule
            }
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
        // Keep death count for display but don't use it for music logic
    }

    public void ResetLayers()
    {
        deathCount = 0;
        activeLayerIndices.Clear();
        UpdateLayersForCurrentLevel();
    }

    public void SetLayerVolume(int layerIndex, float volume)
    {
        if (SoundTrackManager.Instance != null)
        {
            SoundTrackManager.Instance.FadeTrackLayer(currentTrackName, layerIndex, volume, 0f);
        }
    }

    // Custom layer control methods can be added here
    // Example:
    public void SetCustomLayerConfiguration(params int[] layerIndices)
    {
        foreach (var layer in musicLayers)
        {
            DisableLayer(layer.layerIndex);
        }
        
        foreach (int index in layerIndices)
        {
            EnableLayer(index);
        }
    }
} 