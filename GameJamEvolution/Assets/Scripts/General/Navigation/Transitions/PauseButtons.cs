using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtons : MonoBehaviour
{
    public Button resumeButton;
    public Button settingsButton;
    public Button exitButton;

    public GameObject settingsCanvas;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => GameManager.Instance.ResumeGame());
        resumeButton.onClick.AddListener(() => gameObject.SetActive(false));
        settingsButton.onClick.AddListener(() => settingsCanvas.SetActive(true));
        settingsButton.onClick.AddListener(() => gameObject.SetActive(false));
        exitButton.onClick.AddListener(() => LevelManager.Instance.SaveProgress());
        exitButton.onClick.AddListener(() => GameManager.Instance.LoadSceneRequest("MainMenuScene"));
        exitButton.onClick.AddListener(() => GameManager.Instance.ResumeGame());
        exitButton.onClick.AddListener(() => gameObject.SetActive(false));
        resumeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }
}
