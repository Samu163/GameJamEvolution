using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public CameraTweening cameraTweening;
    public static LevelManager Instance;
    public List<LevelData> levels;
    public GridSystem gridSystem;
    public PlayerController player;
    public List<Obstacle> obstaclesOnCurrentLevel;
    public GroupInstantiatorManager groupInstantiatorManager;
    public FallingPlatformsManager fallingPlatformsManager;
    public LevelFinisher levelFinisher;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverCanvas;
    public LevelTimer levelTimer;
    public int levelCount;
    public DissolveManager dissolveManager;

    //Delegates for finish level
    public delegate void OnLevelFinished();
    public OnLevelFinished onLevelFinished;
    public OnLevelFinished restartLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        obstaclesOnCurrentLevel = new List<Obstacle>();
        levelCount = 0;
        onLevelFinished += InitLevel;
        restartLevel += RestartLevel;
    }

    private int currentLevelIndex { get; set; } = 0;

    public void SpawnLevelObstacles(int index)
    {
        PlayerPathChecker pathChecker = new PlayerPathChecker(gridSystem);
        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int end = new Vector2Int(gridSystem.gridWidth - 1, gridSystem.gridHeight - 1);
        int levelIndex;

        if (levelCount == 0)
        {
            levelIndex = Random.Range(4, levels.Count);
        }
        else
        {
            levelIndex = Random.Range(0, levels.Count);
        }

        var obstaclePrefab = levels[levelIndex].obstaclesToSpawn[0];

        
        bool placedSuccessfully = false;
        int maxAttempts = 10; 
        int attempts = 0;

        while (!placedSuccessfully && attempts < maxAttempts)
        {
            if (obstaclePrefab.groupObstacle)
            {
                groupInstantiatorManager.InstantiateGroupObstacles(obstaclePrefab, gridSystem);

                if (pathChecker.IsPathClear(start, end))
                {
                    placedSuccessfully = true;
                }
                else
                {
                    Debug.LogWarning($"Grupo {obstaclePrefab.name} bloqueó el camino. Reintentando...");
                    groupInstantiatorManager.DestroyLastGroup(gridSystem);
                }
            }
            else
            {
                Obstacle obstacleInstance = Instantiate(obstaclePrefab, Vector3.zero, Quaternion.identity);

                bool placed = obstacleInstance.Init(gridSystem);

                if (placed)
                {
                    if (pathChecker.IsPathClear(start, end))
                    {
                        placedSuccessfully = true;
                        obstaclesOnCurrentLevel.Add(obstacleInstance);
                    }
                    else
                    {
                        Debug.LogWarning($"Obstáculo {obstaclePrefab.name} bloqueó el camino. Reintentando...");
                        DestroyObstacle(obstacleInstance.gridPos, obstacleInstance.size);
                    }
                }
                else
                {
                    Debug.LogWarning($"No se pudo colocar el obstáculo {obstaclePrefab.name}. No hay posiciones válidas.");
                    Destroy(obstacleInstance.gameObject);
                }
            }

            attempts++;
        }

        if (!placedSuccessfully)
        {
            Debug.LogError($"No se pudo colocar el obstáculo o grupo {obstaclePrefab.name} después de {maxAttempts} intentos.");
        }
        
    }


    public void GameOver()
    {
        scoreText.text = "Score: " + levelCount;
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    public void FinishLevel()
    {
        levelFinisher.EnableCollider(false);
        cameraTweening.DOCameraAnimation(onLevelFinished);    
    }
    private void InitLevel()
    {

        SpawnLevelObstacles(levelCount);
        player.RespawnPlayer();
        levelFinisher.EnableCollider(true);
        levelCount++;
    }

    public void RestartGame()
    {
        levelCount = 0;
        Time.timeScale = 1;
        gameOverCanvas.SetActive(false);
        cameraTweening.DOCameraAnimation(onLevelFinished);
        gridSystem.DestroyAllObstacles();

        while (obstaclesOnCurrentLevel.Count > 0)
        {
            Destroy(obstaclesOnCurrentLevel[0].gameObject);
            obstaclesOnCurrentLevel.RemoveAt(0);
        }

        levelTimer.timeRemaining = 20;

    }

    public void DestroyObstacle(Vector2Int gridPosition, Vector2Int size)
    {

        for (int i = 0; i < obstaclesOnCurrentLevel.Count; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    if (i >= obstaclesOnCurrentLevel.Count) return;
                    Vector2Int obstacleGridPos = gridPosition + new Vector2Int(j, k);
                    if (obstaclesOnCurrentLevel[i].gridPos == obstacleGridPos)
                    {
                        Obstacle obstacle = obstaclesOnCurrentLevel[i];
                        obstaclesOnCurrentLevel.RemoveAt(i);

                        dissolveManager.StartDissolve(obstacle.gameObject, () =>
                        {
                            gridSystem.DestroyObstacle(obstacleGridPos, obstacle.size);
                            Destroy(obstacle.gameObject);
                        });

                        return;
                    }
                }
            }

        }
    }

    private void RestartLevel()
    {
    }



}
