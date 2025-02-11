using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{

    public Button newGameButton;
    public Button continueButton;
    public Button settingsButton;
    public Button exitButton;
    public Button saveNameButton;
    public Button showLeaderBoardButton;
    public Button howToPlayButton;
    public Button closeButton;
    public Button closeSettingsButton;
    public UIAnimatorManager uiAnimatorManager;
    public LeaderboardsMenu leaderboardsMenu;
    public FadeInController fadeInController;
    public GameObject cartera;
    public Transform carteraPos;

    private Vector3 originalScale;
    [SerializeField] private GameObject Title;
    [SerializeField] private Animator animatorCamera;
    [SerializeField] private GameObject nameBg;
    [SerializeField] private RectTransform leaderBoard;
    [SerializeField] private RectTransform leaderBoardMemebers;
    [SerializeField] private RectTransform nameMenu;
    [SerializeField] private RectTransform howToPlay;
    [SerializeField] private RectTransform Settings;
    [SerializeField] private TMP_InputField usernameInput = null;
    private float originalHowToPlayPositionY;
    private float originalSettingsPositionY;

    [Header("Audio Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    private SoundController soundController;

    private void Awake()
    {
        fadeInController.fadeImage.gameObject.SetActive(false);
        originalScale = howToPlay.transform.localScale;
        originalHowToPlayPositionY = howToPlay.anchoredPosition.y;
        originalSettingsPositionY = Settings.anchoredPosition.y;
        nameBg.SetActive(false);
        howToPlay.gameObject.SetActive(false);
        Settings.gameObject.SetActive(false);
        leaderBoard.gameObject.SetActive(false);
        
        // Initialize sound controller
        soundController = FindObjectOfType<SoundController>();
        if (soundController == null)
        {
            Debug.LogWarning("SoundController not found in scene!");
        }
        
        // Verify slider references
        if (masterVolumeSlider == null || musicVolumeSlider == null || sfxVolumeSlider == null)
        {
            Debug.LogError("One or more volume sliders are not assigned in MenuButtons!");
            return;
        }

        // Initialize slider values and add listeners immediately
        InitializeAudioSliders();
        
        uiAnimatorManager.StartmenuAnim().onComplete += AddListeners;
    }

    private void InitializeAudioSliders()
    {
        if (soundController != null)
        {
            Debug.Log("Initializing audio sliders...");
            
            // Remove any existing listeners first to prevent duplicates
            masterVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();

            // Set initial values from SoundController
            masterVolumeSlider.value = soundController.GetMasterVolume();
            musicVolumeSlider.value = soundController.GetMusicVolume();
            sfxVolumeSlider.value = soundController.GetSFXVolume();

            Debug.Log($"Initial slider values - Master: {masterVolumeSlider.value}, Music: {musicVolumeSlider.value}, SFX: {sfxVolumeSlider.value}");

            // Add listeners
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        else
        {
            Debug.LogError("SoundController is null during slider initialization!");
        }
    }

    private void OnMasterVolumeChanged(float value)
    {
        Debug.Log($"Master slider value changed to: {value}");
        if (soundController != null)
        {
            soundController.SetMasterVolume(value);
        }
        else
        {
            Debug.LogError("SoundController is null when trying to change master volume!");
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        Debug.Log($"Music slider value changed to: {value}");
        if (soundController != null)
        {
            soundController.SetMusicVolume(value);
        }
        else
        {
            Debug.LogError("SoundController is null when trying to change music volume!");
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        Debug.Log($"SFX slider value changed to: {value}");
        if (soundController != null)
        {
            soundController.SetSFXVolume(value);
        }
        else
        {
            Debug.LogError("SoundController is null when trying to change SFX volume!");
        }
    }

    private void AddListeners()
    {
        
        if (GameManager.Instance.CheckSaveData())
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
        newGameButton.onClick.AddListener(() => ShowNamePanel());

        continueButton.onClick.AddListener(() => LoadGame());
        continueButton.onClick.AddListener(() => GameManager.Instance.isLoadingGame = true);
        settingsButton.onClick.AddListener(() => ShowSettings());
        showLeaderBoardButton.onClick.AddListener(() => ShowLeaderBoard());
        howToPlayButton.onClick.AddListener(() => ShowHowToPlay());
        closeButton.onClick.AddListener(() => CloseHowToPlay());
        closeSettingsButton.onClick.AddListener(() => CloseSettings());
        exitButton.onClick.AddListener(() => Application.Quit());
    }

    private async void ShowNamePanel()
    {
        string cloudPlayerName = await GameManager.Instance.GetPlayerNameFromCloud();

        if (!string.IsNullOrEmpty(cloudPlayerName) && cloudPlayerName != "Guest")
        {
            Debug.Log($"Player already has a name in the cloud: {cloudPlayerName}");
            GameManager.Instance.LoadSceneRequest("LevelSelector");
            return;
        }


        nameBg.SetActive(true);
        uiAnimatorManager.AnimateTitle(0, nameMenu).onComplete += () =>
            saveNameButton.onClick.AddListener(() => RegisterPlayerAndLoadLevelSelector());
    }

    public void LoadGame()
    {
        FadeInController.instance.StartFadeIn(() =>
        {
            fadeInController.ResetAlpha(0, false);
            GameManager.Instance.LoadSceneRequest("LevelSelector");
        });
    }


    private void ShowLeaderBoard()
    {
        animatorCamera.SetBool("isLookingTable",true);

        leaderBoardMemebers.gameObject.SetActive(false);
        leaderBoard.gameObject.SetActive(true);
        uiAnimatorManager.AnimateLeaderBoard(1.0f, leaderBoard).onComplete += () =>
        {
            leaderboardsMenu.Open();
            leaderBoardMemebers.gameObject.SetActive(true);
        };
        //uiAnimatorManager.AnimateTitle(0, leaderBoard);
        HideAllButtons(false);
    }

    private void ShowHowToPlay()
    {
        animatorCamera.SetBool("isLookingPost", true);
        StartCoroutine(waitToLookPostHowToPlay());
        HideAllButtonsExceptClose(false);
    }
    private void ShowSettings()
    {
        animatorCamera.SetBool("isLookingPost", true);
        StartCoroutine(waitToLookPostSettings());
        HideAllButtonsExceptClose(false);
    }
    private void CloseHowToPlay()
    {
        StartCoroutine(waitClosePost());
        

        float originalPositionY = howToPlay.anchoredPosition.y;
        Vector2 startPosition = howToPlay.anchoredPosition;
        startPosition.y = 500.0f;

        howToPlay.DOAnchorPosY(originalPositionY - 30f, 0.25f)
                 .SetEase(Ease.OutSine)
                 .OnComplete(() =>
                 {
                     howToPlay.DOAnchorPosY(startPosition.y, 0.5f)
                              .SetEase(Ease.InSine);
                 });
    }
    private void CloseSettings()
    {
        StartCoroutine(waitClosePost());
        

        float originalPositionY = Settings.anchoredPosition.y;
        Vector2 startPosition = Settings.anchoredPosition;
        startPosition.y = 500.0f;

        Settings.DOAnchorPosY(originalPositionY - 30f, 0.25f)
                 .SetEase(Ease.OutSine)
                 .OnComplete(() =>
                 {
                     Settings.DOAnchorPosY(startPosition.y, 0.5f)
                              .SetEase(Ease.InSine);
                 });
    }
    private void RegisterPlayerAndLoadLevelSelector()
    {
        string playerName = usernameInput.text;

        if (string.IsNullOrEmpty(playerName) || playerName.Length < 3)
        {
            Debug.LogError("Player name must be at least 3 characters.");
            return;
        }
        if (string.IsNullOrEmpty(playerName) || playerName.Length > 10)
        {
            Debug.LogError("Player name must have less than 10 characters.");
            return;
        }

        
        GameManager.Instance.SavePlayerName(playerName); 
        GameManager.Instance.RegisterPlayerToLeaderboard(playerName); 
        GameManager.Instance.LoadSceneRequest("LevelSelector"); 
    }
    public void HideAllButtons(bool condition)
    {
        newGameButton.gameObject.SetActive(condition);
        continueButton.gameObject.SetActive(condition);
        settingsButton.gameObject.SetActive(condition);
        exitButton.gameObject.SetActive(condition);
        saveNameButton.gameObject.SetActive(condition);
        showLeaderBoardButton.gameObject.SetActive(condition);
        howToPlayButton.gameObject.SetActive(condition);
        closeButton.gameObject.SetActive(condition);
        closeSettingsButton.gameObject.SetActive(condition);
        Title.gameObject.SetActive(condition);
    }

    public void HideAllButtonsExceptClose(bool condition)
    {
        newGameButton.gameObject.SetActive(condition);
        continueButton.gameObject.SetActive(condition);
        settingsButton.gameObject.SetActive(condition);
        exitButton.gameObject.SetActive(condition);
        saveNameButton.gameObject.SetActive(condition);
        showLeaderBoardButton.gameObject.SetActive(condition);
        howToPlayButton.gameObject.SetActive(condition);
        Title.gameObject.SetActive(condition);
    }

    IEnumerator waitToLookPostHowToPlay()
    {
        yield return new WaitForSeconds(0.5f);
        howToPlay.gameObject.transform.localScale = originalScale;

        Vector2 startPosition = howToPlay.anchoredPosition;
        startPosition.y = 500.0f;
        howToPlay.anchoredPosition = startPosition;

        howToPlay.DOAnchorPosY(originalHowToPlayPositionY - 30.0f, 0.6f)
                 .SetEase(Ease.OutSine)
                 .SetDelay(0)
                 .OnComplete(() =>
                 {
                     howToPlay.DOAnchorPosY(originalHowToPlayPositionY, 0.15f)
                              .SetEase(Ease.InSine);
                 });


        howToPlay.gameObject.SetActive(true);
    }

    IEnumerator waitToLookPostSettings()
    {
        yield return new WaitForSeconds(0.5f);
        Settings.gameObject.transform.localScale = originalScale;
        Vector2 startPosition = Settings.anchoredPosition;
        startPosition.y = 500.0f;
        Settings.anchoredPosition = startPosition;

        Settings.DOAnchorPosY(originalSettingsPositionY - 30.0f, 0.6f)
                 .SetEase(Ease.OutSine)
                 .SetDelay(0)
                 .OnComplete(() =>
                 {
                     Settings.DOAnchorPosY(originalSettingsPositionY, 0.15f)
                              .SetEase(Ease.InSine);
                 });


        Settings.gameObject.SetActive(true);
    }

    IEnumerator waitClosePost()
    {
        yield return new WaitForSeconds(1f);
        animatorCamera.SetBool("isLookingPost", false);
        yield return new WaitForSeconds(1f);
        HideAllButtonsExceptClose(true);
    }
}
