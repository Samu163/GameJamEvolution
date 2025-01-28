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
    public UIAnimatorManager uiAnimatorManager;
    public LeaderboardsMenu leaderboardsMenu;

    [SerializeField] private GameObject nameBg;
    [SerializeField] private RectTransform leaderBoard;
    [SerializeField] private RectTransform nameMenu;
    [SerializeField] private TMP_InputField usernameInput = null;

    private void Awake()
    {
        nameBg.SetActive(false);
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
        exitButton.onClick.AddListener(() => Application.Quit());
    }

    private void ShowNamePanel()
    {
        nameBg.SetActive(true);
        uiAnimatorManager.AnimateTitle(0, nameMenu).onComplete += () => saveNameButton.onClick.AddListener(() => RegisterPlayerAndLoadLevelSelector());
    }

    private void ShowLeaderBoard()
    {
        leaderboardsMenu.Open();
        uiAnimatorManager.AnimateTitle(0, leaderBoard);
    }

    private void RegisterPlayerAndLoadLevelSelector()
    {
        string playerName = usernameInput.text;

        if (string.IsNullOrEmpty(playerName) || playerName.Length < 3)
        {
            Debug.LogError("Player name must be at least 3 characters.");
            return;
        }

        GameManager.Instance.SavePlayerName(playerName); 
        GameManager.Instance.RegisterPlayerToLeaderboard(playerName); 
        GameManager.Instance.LoadSceneRequest("LevelSelector"); 
    }


}
