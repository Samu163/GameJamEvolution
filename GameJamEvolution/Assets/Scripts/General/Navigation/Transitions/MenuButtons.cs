using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{

    public Button newGameButton;
    public Button continueButton;
    public Button settingsButton;
    public Button exitButton;
    public UIAnimatorManager uiAnimatorManager;

    private void Awake()
    {
        uiAnimatorManager.StartmenuAnim().onComplete += AddListeners;
    }
    private void AddListeners()
    {
        newGameButton.onClick.AddListener(() => GameManager.Instance.LoadSceneRequest("GameScene"));
        if (GameManager.Instance.CheckSaveData())
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
        continueButton.onClick.AddListener(() => GameManager.Instance.LoadSceneRequest("GameScene"));
        continueButton.onClick.AddListener(() => GameManager.Instance.isLoadingGame = true);
        settingsButton.onClick.AddListener(() => GameManager.Instance.LoadScreenRequest("SettingsScreen"));
        exitButton.onClick.AddListener(() => Application.Quit());
    }

}
