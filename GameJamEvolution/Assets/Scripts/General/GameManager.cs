using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject canvasForInstantiations;
    public NavigationController navigationController;
    public bool isPaused = false;
    public int sceneID;

    private SaveSystem saveSystem;
    public bool isLoadingGame = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        saveSystem = new SaveSystem();

    }

    public void LoadSceneRequest(string sceneName)
    {
       
        navigationController.LoadScene(sceneName);
    }

    public void LoadScreenRequest(string screenName)
    {
        navigationController.LoadScreen(screenName, canvasForInstantiations.transform);
    }

    public void DestroyScreenRequest(string screenName)
    {
        navigationController.DestroyScreen(screenName);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        
    }

    public bool CheckSaveData()
    {
        return saveSystem.CheckSaveFile();
    }

}
