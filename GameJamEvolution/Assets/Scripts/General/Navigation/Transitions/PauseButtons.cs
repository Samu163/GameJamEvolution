using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtons : MonoBehaviour
{
    public Button resumeButton;
    public Button settingsButton;
    public Button exitButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => GameManager.Instance.ResumeGame());
        settingsButton.onClick.AddListener(() => GameManager.Instance.LoadScreenRequest("SettingsCanvas"));
        settingsButton.onClick.AddListener(() => GameManager.Instance.DestroyScreenRequest("PauseCanvas"));
        exitButton.onClick.AddListener(() => GameManager.Instance.LoadSceneRequest("MainMenuScene"));
        exitButton.onClick.AddListener(() => GameManager.Instance.ResumeGame());
    }
}
