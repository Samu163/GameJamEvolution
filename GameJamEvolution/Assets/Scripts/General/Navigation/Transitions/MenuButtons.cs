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
        switch(GameManager.Instance.sceneID)
        {
            case 0:
                newGameButton.onClick.AddListener(() => GameManager.Instance.LoadSceneRequest("GameScene"));
                break;
            case 1:
                newGameButton.onClick.AddListener(() => GameManager.Instance.LoadSceneRequest("GameScene2"));
                break;
            case 2:
                newGameButton.onClick.AddListener(() => GameManager.Instance.LoadSceneRequest("GameScene3"));
                break;
        }
        
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
