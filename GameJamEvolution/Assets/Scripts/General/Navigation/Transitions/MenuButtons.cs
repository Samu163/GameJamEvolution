using System.Collections;
using System.Collections.Generic;
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
        uiAnimatorManager.StartmenuAnim().onComplete += () => newGameButton.onClick.AddListener(() => GameManager.Instance.LoadSceneRequest("GameScene"));
        continueButton.onClick.AddListener(() => GameManager.Instance.LoadSceneRequest("PlayerTestScene"));
        settingsButton.onClick.AddListener(() => GameManager.Instance.LoadScreenRequest("SettingsScreen"));
        exitButton.onClick.AddListener(() => Application.Quit());       
    }

}
