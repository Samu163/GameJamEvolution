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
            Debug.Log("El nivel está lleno. No se generaron más obstáculos.");
        }
        obstaclesOnCurrentLevel.Add(obstacleInstance);     
    }


    public void InitNewLevel()
    {
        SpawnLevelObstacles(levelCount);
        player.RespawnPlayer();
        levelCount++;
    }
}
