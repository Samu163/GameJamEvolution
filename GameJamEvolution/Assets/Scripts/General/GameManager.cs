using System.Collections;
using System.Collections.Generic;
using Unity.Services.Leaderboards;
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


    public async void RegisterPlayerToLeaderboard(string playerName)
    {
        try
        {
            var metadata = new Dictionary<string, object> { { "PlayerName", playerName } };

            var options = new AddPlayerScoreOptions
            {
                Metadata = metadata
            };

            await LeaderboardsService.Instance.AddPlayerScoreAsync(
                "test", // Reemplaza con tu Leaderboard ID
                0,      // Puntaje inicial (puede ser 0 o algún valor predeterminado)
                options
            );

            Debug.Log($"Player '{playerName}' registered in the leaderboard.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to register player to leaderboard: {ex.Message}");
        }
    }


    public void SavePlayerName(string playerName)
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        Debug.Log($"Player name saved: {playerName}");
    }

    public static string GetPlayerName()
    {
        return PlayerPrefs.GetString("PlayerName", "Guest");
    }
}
