using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISoundController : MonoBehaviour
{
    public static UISoundController Instance { get; private set; }

    [System.Serializable]
    public class UISoundConfig
    {
        public string hoverSound = "UIHover";
        [Range(0f, 1f)] public float hoverVolume = 1f;
        
        public string clickSound = "UIClick";
        [Range(0f, 1f)] public float clickVolume = 1f;
        
        public string backSound = "UIBack";
        [Range(0f, 1f)] public float backVolume = 1f;
        
        public string errorSound = "UIError";
        [Range(0f, 1f)] public float errorVolume = 1f;
        
        public string successSound = "UISuccess";
        [Range(0f, 1f)] public float successVolume = 1f;
    }

    [Header("Sound Configuration")]
    [SerializeField] private UISoundConfig soundConfig;
    
    [Header("Auto-Attach Settings")]
    [SerializeField] private bool autoAttachToButtons = true;
    [SerializeField] private bool playHoverSounds = true;
    [SerializeField] private bool playClickSounds = true;

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
        Debug.Log("UISoundController Start");
        if (autoAttachToButtons)
        {
            AttachToAllUIElements();
        }
    }

    public void AttachToAllUIElements()
    {
        // Find all buttons in the scene and attach handlers
        Button[] buttons = FindObjectsOfType<Button>(true);
        Debug.Log($"Found {buttons.Length} buttons");
        
        foreach (Button button in buttons)
        {
            AttachHandlersToButton(button);
        }
    }

    public void AttachHandlersToButton(Button button)
    {
        if (!button.gameObject.GetComponent<UISoundHandler>())
        {
            Debug.Log($"Attaching handler to button: {button.gameObject.name}");
            UISoundHandler handler = button.gameObject.AddComponent<UISoundHandler>();
            handler.Initialize(this, playHoverSounds, playClickSounds);
        }
    }

    // Add this method to manually attach to a specific button
    public void ManuallyAttachToButton(Button button)
    {
        if (button != null)
        {
            AttachHandlersToButton(button);
        }
    }

    // Add this to help with scene changes
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        if (autoAttachToButtons)
        {
            AttachToAllUIElements();
        }
    }

    // Methods for playing UI sounds
    public void PlayHoverSound()
    {
        if (UISFXManager.Instance != null)
        {
            UISFXManager.Instance.PlaySoundWithVolume(soundConfig.hoverSound, soundConfig.hoverVolume);
        }
    }

    public void PlayClickSound()
    {
        if (UISFXManager.Instance != null)
        {
            UISFXManager.Instance.PlaySoundWithVolume(soundConfig.clickSound, soundConfig.clickVolume);
        }
    }

    public void PlayBackSound()
    {
        if (UISFXManager.Instance != null)
        {
            UISFXManager.Instance.PlaySoundWithVolume(soundConfig.backSound, soundConfig.backVolume);
        }
    }

    public void PlayErrorSound()
    {
        if (UISFXManager.Instance != null)
        {
            UISFXManager.Instance.PlaySoundWithVolume(soundConfig.errorSound, soundConfig.errorVolume);
        }
    }

    public void PlaySuccessSound()
    {
        if (UISFXManager.Instance != null)
        {
            UISFXManager.Instance.PlaySoundWithVolume(soundConfig.successSound, soundConfig.successVolume);
        }
    }

    // Custom sound method for specific UI elements
    public void PlayUISound(string soundName, float volume = 1f)
    {
        if (UISFXManager.Instance != null)
        {
            UISFXManager.Instance.PlaySoundWithVolume(soundName, volume);
        }
    }
} 