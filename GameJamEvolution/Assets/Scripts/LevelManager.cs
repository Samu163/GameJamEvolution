using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public List<LevelData> levels;
    public GridSystem gridSystem;
    public PlayerController player;
    public List<Obstacle> obstaclesOnCurrentLevel;
    public GroupInstantiatorManager groupInstantiatorManager;
    public FallingPlatformsManager fallingPlatformsManager;
    public int levelCount;

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
}

    private int currentLevelIndex { get; set; } = 0;

    public void SpawnLevelObstacles(int index)
    {
        foreach (var obstaclePrefab in levels[index].obstaclesToSpawn)
        {

            if (obstaclePrefab.groupObstacle)
            {
                groupInstantiatorManager.InstantiateGroupObstacles(obstaclePrefab, gridSystem);
                continue;
            }
            else
            {
                Obstacle obstacleInstance = Instantiate(obstaclePrefab, Vector3.zero, Quaternion.identity);

                bool placed = obstacleInstance.Init(gridSystem);
                if (placed)
                {
                    obstaclesOnCurrentLevel.Add(obstacleInstance);
                }
                else
                {
                    Debug.LogWarning($"No se pudo colocar el obstáculo {obstaclePrefab.name}. No hay posiciones válidas.");
                    Destroy(obstacleInstance.gameObject);
                }
            }

            
        }
    }



    public void InitNewLevel()
    {
        SpawnLevelObstacles(levelCount);
        player.RespawnPlayer();
        levelCount++;
    }

    public void DestroyObstacle(Vector2Int gridPosition, Vector2Int size)
    {

        List<Vector2Int> obstacleGridPositions = gridSystem.GetPositionsWithObstacles(new Vector2Int(1, 1));

        for (int i = 0; i < obstaclesOnCurrentLevel.Count; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    Vector2Int obstacleGridPos = gridPosition + new Vector2Int(j, k);
                    if (obstaclesOnCurrentLevel[i].gridPos == obstacleGridPos)
                    {
                        
                        gridSystem.DestroyObstacle(obstacleGridPos, obstaclesOnCurrentLevel[i].size);
                        Destroy(obstaclesOnCurrentLevel[i].gameObject);
                        obstaclesOnCurrentLevel.RemoveAt(i);

                    }
                }
            }

        }
    }

    
}
