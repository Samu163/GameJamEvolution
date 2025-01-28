using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    [System.Serializable]
    public class SceneTrackConfig
    {
        public string sceneName;
        public string trackName;
        [Tooltip("Initial layers to enable when entering this scene")]
        public int[] initialLayers;
    }

    [Header("Scene Configuration")]
    [SerializeField] private List<SceneTrackConfig> sceneConfigs = new List<SceneTrackConfig>();
    [SerializeField] private float sceneCrossFadeTime = 2f;
    
    [Header("Music Configuration")]
    [SerializeField] private string currentTrackName = "Track1";
    
    [Header("Layer Settings")]
    [SerializeField] private List<LayerConfig> musicLayers = new List<LayerConfig>();
    
    [Header("Level Rules")]
    [SerializeField] private List<LevelLayerRule> levelRules = new List<LevelLayerRule>();

    private HashSet<int> activeLayerIndices = new HashSet<int>();
    private string currentSceneName;
    
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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        InitializeSceneMusic(currentSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentSceneName != scene.name)
        {
            currentSceneName = scene.name;
            HandleSceneTransition(scene.name);
        }
    }

    private void HandleSceneTransition(string newSceneName)
    {
        var sceneConfig = sceneConfigs.Find(c => c.sceneName == newSceneName);
        if (sceneConfig != null)
        {
            if (sceneConfig.trackName != currentTrackName)
            {
                // Crossfade to new track
                StartCoroutine(CrossFadeToNewTrack(sceneConfig));
            }
            else
            {
                // Same track, just update layers
                UpdateLayersForScene(sceneConfig);
            }
        }
    }

    private System.Collections.IEnumerator CrossFadeToNewTrack(SceneTrackConfig newConfig)
    {
        // Fade out current track
        if (SoundTrackManager.Instance != null)
        {
            foreach (var layer in musicLayers)
            {
                SoundTrackManager.Instance.FadeTrackLayer(currentTrackName, layer.layerIndex, 0f, sceneCrossFadeTime);
            }
        }

        yield return new WaitForSeconds(sceneCrossFadeTime);

        // Change to new track
        currentTrackName = newConfig.trackName;
        if (SoundTrackManager.Instance != null)
        {
            SoundTrackManager.Instance.PlayTrack(currentTrackName);
            UpdateLayersForScene(newConfig);
        }
    }

    private void InitializeSceneMusic(string sceneName)
    {
        var sceneConfig = sceneConfigs.Find(c => c.sceneName == sceneName);
        if (sceneConfig != null && SoundTrackManager.Instance != null)
        {
            currentTrackName = sceneConfig.trackName;
            SoundTrackManager.Instance.PlayTrack(currentTrackName);
            
            // Initialize layers
            foreach (var layer in musicLayers)
            {
                SoundTrackManager.Instance.FadeTrackLayer(currentTrackName, layer.layerIndex, 0f, 0f);
            }

            // Enable initial layers for this scene
            foreach (int layerIndex in sceneConfig.initialLayers)
            {
                EnableLayer(layerIndex);
            }
        }
    }

    private void UpdateLayersForScene(SceneTrackConfig sceneConfig)
    {
        // Disable all layers first
        foreach (var layer in musicLayers)
        {
            DisableLayer(layer.layerIndex);
        }

        // Enable initial layers for this scene
        foreach (int layerIndex in sceneConfig.initialLayers)
        {
            EnableLayer(layerIndex);
        }

        // If this is a gameplay scene, also apply level rules
        if (sceneConfig.sceneName.Contains("Level") || sceneConfig.sceneName.Contains("Game"))
        {
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