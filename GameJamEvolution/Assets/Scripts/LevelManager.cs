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
        int maxAttempts = 10; // N�mero m�ximo de intentos para colocar cada obst�culo.
        int attempts = 0;

        while (!placedSuccessfully && attempts < maxAttempts)
        {
            if (obstaclePrefab.groupObstacle)
            {
                // Colocar un grupo de obst�culos
                groupInstantiatorManager.InstantiateGroupObstacles(obstaclePrefab, gridSystem);

                // Verificar si el camino est� despejado despu�s de colocar el grupo.
                if (pathChecker.IsPathClear(start, end))
                {
                    placedSuccessfully = true;
                }
                else
                {
                    // Si el camino est� bloqueado, eliminar el grupo.
                    Debug.LogWarning($"Grupo {obstaclePrefab.name} bloque� el camino. Reintentando...");
                    groupInstantiatorManager.DestroyLastGroup(gridSystem);
                }
            }
            else
            {
                // Colocar un obst�culo individual.
                Obstacle obstacleInstance = Instantiate(obstaclePrefab, Vector3.zero, Quaternion.identity);

                bool placed = obstacleInstance.Init(gridSystem);

                if (placed)
                {
                    // Verificar si el camino est� despejado despu�s de colocarlo.
                    if (pathChecker.IsPathClear(start, end))
                    {
                        placedSuccessfully = true;
                        obstaclesOnCurrentLevel.Add(obstacleInstance);
                    }
                    else
                    {
                        // Si el camino est� bloqueado, eliminar el obst�culo.
                        Debug.LogWarning($"Obst�culo {obstaclePrefab.name} bloque� el camino. Reintentando...");
                        DestroyObstacle(obstacleInstance.gridPos, obstacleInstance.size);
                    }
                }
                else
                {
                    // Si no se pudo colocar inicialmente.
                    Debug.LogWarning($"No se pudo colocar el obst�culo {obstaclePrefab.name}. No hay posiciones v�lidas.");
                    Destroy(obstacleInstance.gameObject);
                }
            }

            attempts++;
        }

        if (!placedSuccessfully)
        {
            Debug.LogError($"No se pudo colocar el obst�culo o grupo {obstaclePrefab.name} despu�s de {maxAttempts} intentos.");
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
                        
                        gridSystem.DestroyObstacle(obstacleGridPos, obstaclesOnCurrentLevel[i].size);
                        Destroy(obstaclesOnCurrentLevel[i].gameObject);
                        obstaclesOnCurrentLevel.RemoveAt(i);

                    }
                }
            }

        }
    }

    
}
