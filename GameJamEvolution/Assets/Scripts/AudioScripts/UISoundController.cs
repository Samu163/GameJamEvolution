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
        if (autoAttachToButtons)
        {
            AttachToAllUIElements();
        }
    }

    public void AttachToAllUIElements()
    {
        // Find all buttons in the scene and attach handlers
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button button in buttons)
        {
            AttachHandlersToButton(button);
        }
    }

    public void AttachHandlersToButton(Button button)
    {
        if (!button.gameObject.GetComponent<UISoundHandler>())
        {
            UISoundHandler handler = button.gameObject.AddComponent<UISoundHandler>();
            handler.Initialize(this, playHoverSounds, playClickSounds);
        }
    }

    // Methods for playing UI sounds
    public void PlayHoverSound()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySoundWithVolume(soundConfig.hoverSound, soundConfig.hoverVolume);
        }
    }

    public void PlayClickSound()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySoundWithVolume(soundConfig.clickSound, soundConfig.clickVolume);
        }
    }

    public void PlayBackSound()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySoundWithVolume(soundConfig.backSound, soundConfig.backVolume);
        }
    }

    public void PlayErrorSound()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySoundWithVolume(soundConfig.errorSound, soundConfig.errorVolume);
        }
    }

    public void PlaySuccessSound()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySoundWithVolume(soundConfig.successSound, soundConfig.successVolume);
        }
    }

    // Custom sound method for specific UI elements
    public void PlayUISound(string soundName, float volume = 1f)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySoundWithVolume(soundName, volume);
        }
    }
} 