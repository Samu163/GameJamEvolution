using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public List<LevelData> levels;
    public GridSystem gridSystem;
    public PlayerController player;
    private List<Obstacle> obstaclesOnCurrentLevel;
    private int levelCount;

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
        Obstacle obstaclePrefab = levels[index].obstaclesToSpawn[Random.Range(0, levels[index].obstaclesToSpawn.Count)];
        var obstacleInstance = Instantiate(obstaclePrefab);
        bool placed = obstacleInstance.Init(gridSystem);
        if (!placed)
        {
            Debug.Log("El nivel est� lleno. No se generaron m�s obst�culos.");
        }
        obstaclesOnCurrentLevel.Add(obstacleInstance);
        obstacleInstance.id = obstaclesOnCurrentLevel.Count;
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
                        gridSystem.DestroyObstacle(obstacleGridPos, size);
                        Destroy(obstaclesOnCurrentLevel[i].gameObject);
                        obstaclesOnCurrentLevel.RemoveAt(i);

                    }
                }
            }

        }
    }

    
}
