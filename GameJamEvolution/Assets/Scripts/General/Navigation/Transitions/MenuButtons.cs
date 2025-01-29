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
    public UIAnimatorManager uiAnimatorManager;
    public LeaderboardsMenu leaderboardsMenu;
    private Vector3 originalScale;

    [SerializeField] private GameObject nameBg;
    [SerializeField] private RectTransform leaderBoard;
    [SerializeField] private RectTransform nameMenu;
    [SerializeField] private RectTransform howToPlay;
    [SerializeField] private TMP_InputField usernameInput = null;

    private void Awake()
    {
        originalScale = howToPlay.transform.localScale;
        nameBg.SetActive(false);
        howToPlay.gameObject.SetActive(false);
        leaderBoard.gameObject.SetActive(false);
        uiAnimatorManager.StartmenuAnim().onComplete += AddListeners;
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
        continueButton.onClick.AddListener(() => GameManager.Instance.LoadSceneRequest("GameScene"));
        continueButton.onClick.AddListener(() => GameManager.Instance.isLoadingGame = true);
        settingsButton.onClick.AddListener(() => GameManager.Instance.LoadScreenRequest("SettingsScreen"));
        showLeaderBoardButton.onClick.AddListener(() => ShowLeaderBoard());
        howToPlayButton.onClick.AddListener(() => ShowHowToPlay());
        closeButton.onClick.AddListener(() => CloseHowToPlay());
        exitButton.onClick.AddListener(() => Application.Quit());
    }

    private async void ShowNamePanel()
    {
        //string cloudPlayerName = await GameManager.Instance.GetPlayerNameFromCloud();

        //if (!string.IsNullOrEmpty(cloudPlayerName) && cloudPlayerName != "Guest")
        //{
        //    Debug.Log($"Player already has a name in the cloud: {cloudPlayerName}");
        //    GameManager.Instance.LoadSceneRequest("LevelSelector");
        //    return;
        //}


        nameBg.SetActive(true);
        uiAnimatorManager.AnimateTitle(0, nameMenu).onComplete += () =>
            saveNameButton.onClick.AddListener(() => RegisterPlayerAndLoadLevelSelector());
    }

    private void ShowLeaderBoard()
    {
        leaderboardsMenu.Open();
        uiAnimatorManager.AnimateTitle(0, leaderBoard);
    }

    private void ShowHowToPlay()
    {
        howToPlay.gameObject.transform.localScale = originalScale;
        uiAnimatorManager.AnimateTitle(0, howToPlay);
        howToPlay.gameObject.SetActive(true);
    }

    private void CloseHowToPlay()
    {
        //howToPlay.gameObject.transform.DOScale(originalScale * 0, 0.5f).SetEase(Ease.OutBack);

        float originalPositionY = howToPlay.anchoredPosition.y;
        Vector2 startPosition = howToPlay.anchoredPosition;
        startPosition.y = 500.0f;

        howToPlay.DOAnchorPosY(originalPositionY - 30f, 0.5f)
                 .SetEase(Ease.OutSine)
                 .OnComplete(() =>
                 {
                     howToPlay.DOAnchorPosY(startPosition.y, 1f)
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


}
