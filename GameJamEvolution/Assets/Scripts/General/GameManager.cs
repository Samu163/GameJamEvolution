using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using Unity.Services.Core;
using Unity.Services.Authentication;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject canvasForInstantiations;
    public NavigationController navigationController;
    public bool isPaused = false;
    public int sceneID;

    private SaveSystem saveSystem;
    public bool isLoadingGame = false;

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            // Inicializa Unity Services e intenta autenticar al jugador
            await InitializeUnityServicesAndAuthenticate();
        }

        saveSystem = new SaveSystem();
    }

    private async Task InitializeUnityServicesAndAuthenticate()
    {
        try
        {
            // Inicializa Unity Services
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services initialized successfully.");

            // Autentica al jugador
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync(); // Inicia sesión anónima
                Debug.Log($"Player authenticated successfully with ID: {AuthenticationService.Instance.PlayerId}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to initialize or authenticate: {ex.Message}");
        }
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
    public async void UpdatePlayerScore(int levelCount)
    {
        try
        {
            var scores = await LeaderboardsService.Instance.GetPlayerScoreAsync("test");

            var currentCloudScore = scores?.Score ?? 0; 

            // Comparar puntuaciones
            if (levelCount > currentCloudScore)
            {
                await AddPlayerScoreToCloud(levelCount);
            }
            else
            {
                Debug.Log("No new high score. Cloud score remains: " + currentCloudScore);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error updating player score: " + e.Message);
        }
    }

    private async Task AddPlayerScoreToCloud(int score)
    {
        try
        {
            string playerName = PlayerPrefs.GetString("PlayerName", "Guest");

            var metadata = new Dictionary<string, object>
            {
                { "PlayerName", playerName }
            };

            await LeaderboardsService.Instance.AddPlayerScoreAsync(
                "test",
                score,
                new AddPlayerScoreOptions { Metadata = metadata }
            );

            Debug.Log($"Updated leaderboard with score: {score} for player: {playerName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to add player score: " + e.Message);
        }
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
                "test", 
                0,      
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
