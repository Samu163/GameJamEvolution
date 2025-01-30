using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static SaveData;

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
    public DestroyManager destroyManager;
    public Volume globalVolume; 
    public Volume globalVolumeOliver; 
    public TextMeshProUGUI respawnText;

    //Delegates for finish level
    public delegate void OnLevelFinished();
    public OnLevelFinished onLevelFinished;
    public OnLevelFinished restartLevel;

    private SaveSystem saveSystem;
    private SaveData saveData;

    [Header("Obstacles")]
    public Ground groundPrefab;
    public SlidingGround slidingGroundPrefab;
    public FallingPlatform fallingPlatformPrefab;
    public FallingLamp lampPrefab;
    public Closet closetPrefab;
    public Cuadro cuadroPrefab;
    public Cuadro cuadroLaserPrefab;
    public Aspiradora aspiradoraPrefab;
    public Clock clockPrefab;
    public Spike spikePrefab;


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
        
        if (globalVolume != null) globalVolume.gameObject.SetActive(false);
        if (respawnText != null) respawnText.gameObject.SetActive(false);
        obstaclesOnCurrentLevel = new List<Obstacle>();
        levelCount = 0;
        onLevelFinished += InitLevel;
        restartLevel += RestartLevel;
        saveSystem = new SaveSystem();
    }

    private void Start()
    {
        FadeInController.instance.fadeImage.gameObject.SetActive(true);
        StartCoroutine(waitToLagForFade());
        if (GameManager.Instance.isLoadingGame)
        {
            LoadProgress();
            GameManager.Instance.isLoadingGame = false;
        }

        CreateLevel(GameManager.Instance.sceneID);
    }

    IEnumerator waitToLagForFade()
    {
        yield return new WaitForSeconds(2f);
        FadeInController.instance.StartFadeOut(()=>FadeInController.instance.fadeImage.gameObject.SetActive(false));
    }

    public void LoadProgress()
    {
        if (saveSystem == null)
        {
            saveSystem = new SaveSystem();
        }

        saveData = saveSystem.Load();
        if (saveData == null)
        {
            saveData = new SaveData
            {
                timeRemaining = levelTimer.timeRemaining,
                destroyCharge = destroyManager.rechargeValue,
            };
        }

        levelTimer.timeRemaining = saveData.timeRemaining;
        destroyManager.rechargeValue = saveData.destroyCharge;
        levelCount = saveData.levelCount;
        GameManager.Instance.sceneID = saveData.sceneID;

        for (int i = 0; i < gridSystem.gridWidth; i++)
        {
            for (int j = 0; j < gridSystem.gridHeight; j++)
            {
                var cellData = saveData.cells[i * gridSystem.gridHeight + j];
                gridSystem.GetGrid()[i, j].isOcupied = cellData.isOccupied;
                gridSystem.GetGrid()[i, j].type = cellData.type;
            }
        }

        for (int i = 0; i < saveData.obstaclesInGrid.Count; i++)
        {
            var obstacleData = saveData.obstaclesInGrid[i];
            
            switch(obstacleData.type)
            {
                case Obstacle.ObstacleType.FallingPlatform:
                    var fallingPlatform = Instantiate(fallingPlatformPrefab, Vector3.zero, Quaternion.identity);
                    fallingPlatform.size = obstacleData.size;
                    fallingPlatform.gridPos = obstacleData.gridPos;
                    fallingPlatform.cellType = obstacleData.cellType;
                    fallingPlatform.isFallingPlatform = obstacleData.isFallingPlatform;
                    fallingPlatform.type = obstacleData.type;
                    Vector3 worldPosition = gridSystem.GridToWorldPosition(fallingPlatform.gridPos);
                    fallingPlatform.transform.position = worldPosition;
                    break;
                case Obstacle.ObstacleType.Ground:
                    var ground = Instantiate(groundPrefab, Vector3.zero, Quaternion.identity);
                    ground.size = obstacleData.size;
                    ground.gridPos = obstacleData.gridPos;
                    ground.cellType = obstacleData.cellType;
                    ground.isFallingPlatform = obstacleData.isFallingPlatform;
                    ground.type = obstacleData.type;
                    Vector3 worldPositionGround = gridSystem.GridToWorldPosition(ground.gridPos);
                    ground.transform.position = worldPositionGround;
                    break;
                case Obstacle.ObstacleType.SlidingGround:
                    var slidingGround = Instantiate(slidingGroundPrefab, Vector3.zero, Quaternion.identity);
                    slidingGround.size = obstacleData.size;
                    slidingGround.gridPos = obstacleData.gridPos;
                    slidingGround.cellType = obstacleData.cellType;
                    slidingGround.isFallingPlatform = obstacleData.isFallingPlatform;
                    slidingGround.type = obstacleData.type;
                    Vector3 worldPositionSlidingGround = gridSystem.GridToWorldPosition(slidingGround.gridPos);
                    slidingGround.transform.position = worldPositionSlidingGround;
                    break;
                case Obstacle.ObstacleType.Lamp:
                    var lamp = Instantiate(lampPrefab, Vector3.zero, Quaternion.identity);
                    lamp.size = obstacleData.size;
                    lamp.gridPos = obstacleData.gridPos;
                    lamp.cellType = obstacleData.cellType;
                    lamp.isFallingPlatform = obstacleData.isFallingPlatform;
                    lamp.type = obstacleData.type;
                    Vector3 worldPositionLamp = gridSystem.GridToWorldPosition(lamp.gridPos);
                    lamp.transform.position = worldPositionLamp;
                    break;
                case Obstacle.ObstacleType.Closet:
                    var closet = Instantiate(closetPrefab, Vector3.zero, Quaternion.identity);
                    closet.size = obstacleData.size;
                    closet.gridPos = obstacleData.gridPos;
                    closet.cellType = obstacleData.cellType;
                    closet.isFallingPlatform = obstacleData.isFallingPlatform;
                    closet.type = obstacleData.type;
                    Vector3 worldPositionCloset = gridSystem.GridToWorldPosition(closet.gridPos);
                    closet.transform.position = worldPositionCloset;
                    break;
                case Obstacle.ObstacleType.Cuadro:
                    
                    if (obstacleData.isLaser)
                    {
                        var cuadro = Instantiate(cuadroLaserPrefab, Vector3.zero, Quaternion.identity);
                        cuadro.size = obstacleData.size;
                        cuadro.gridPos = obstacleData.gridPos;
                        cuadro.cellType = obstacleData.cellType;
                        cuadro.isFallingPlatform = obstacleData.isFallingPlatform;
                        cuadro.type = obstacleData.type;
                        Vector3 worldPositionCuadro = gridSystem.GridToWorldPosition(cuadro.gridPos);
                        cuadro.transform.position = worldPositionCuadro;
                    }
                    else
                    {
                        var cuadro = Instantiate(cuadroPrefab, Vector3.zero, Quaternion.identity);
                        cuadro.size = obstacleData.size;
                        cuadro.gridPos = obstacleData.gridPos;
                        cuadro.cellType = obstacleData.cellType;
                        cuadro.isFallingPlatform = obstacleData.isFallingPlatform;
                        cuadro.type = obstacleData.type;
                        Vector3 worldPositionCuadro = gridSystem.GridToWorldPosition(cuadro.gridPos);
                        cuadro.transform.position = worldPositionCuadro;
                    }
                    
                    break;
                case Obstacle.ObstacleType.Aspiradora:
                    var aspiradora = Instantiate(aspiradoraPrefab, Vector3.zero, Quaternion.identity);
                    aspiradora.size = obstacleData.size;
                    aspiradora.gridPos = obstacleData.gridPos;
                    aspiradora.cellType = obstacleData.cellType;
                    aspiradora.isFallingPlatform = obstacleData.isFallingPlatform;
                    aspiradora.type = obstacleData.type;
                    Vector3 worldPositionAspiradora = gridSystem.GridToWorldPosition(aspiradora.gridPos);
                    aspiradora.transform.position = worldPositionAspiradora;
                    break;
                case Obstacle.ObstacleType.Clock:
                    var clock = Instantiate(clockPrefab, Vector3.zero, Quaternion.identity);
                    clock.size = obstacleData.size;
                    clock.gridPos = obstacleData.gridPos;
                    clock.cellType = obstacleData.cellType;
                    clock.isFallingPlatform = obstacleData.isFallingPlatform;
                    clock.type = obstacleData.type;
                    Vector3 worldPositionClock = gridSystem.GridToWorldPosition(clock.gridPos);
                    clock.transform.position = worldPositionClock;
                    break;
                case Obstacle.ObstacleType.Spike:
                    var spike = Instantiate(spikePrefab, Vector3.zero, Quaternion.identity);
                    spike.size = obstacleData.size;
                    spike.gridPos = obstacleData.gridPos;
                    spike.cellType = obstacleData.cellType;
                    spike.isFallingPlatform = obstacleData.isFallingPlatform;
                    spike.type = obstacleData.type;
                    Vector3 worldPositionSpike = gridSystem.GridToWorldPosition(spike.gridPos);
                    spike.transform.position = worldPositionSpike;
                    break;
            }
        }

        

        
        

        Debug.Log("Progress Loaded | Time Remaining: " + saveData.timeRemaining);
    }

    public void SaveProgress()
    {
        if (saveSystem == null)
        {
            saveSystem = new SaveSystem();
        }

        if (saveData == null)
        {
            saveData = new SaveData
            {
                timeRemaining = levelTimer.timeRemaining,
                destroyCharge = destroyManager.rechargeValue,
            };
        }

        saveData.timeRemaining = levelTimer.timeRemaining;
        saveData.destroyCharge = destroyManager.rechargeValue;
        saveData.levelCount = levelCount;
        saveData.sceneID = GameManager.Instance.sceneID;

        for (int i = 0; i < obstaclesOnCurrentLevel.Count; i++)
        {
            var obstacle = obstaclesOnCurrentLevel[i];
            saveData.obstaclesInGrid.Add(new SaveData.ObstacleData
            {
                size = obstacle.size,
                gridPos = obstacle.gridPos,
                cellType = obstacle.cellType,
                type = obstacle.type,
                isFallingPlatform = obstacle.isFallingPlatform
            });
        }

        for (int i = 0; i < gridSystem.gridWidth; i++)
        {
            for (int j = 0; j < gridSystem.gridHeight; j++)
            {
                var cell = gridSystem.GetGrid()[i, j];
                saveData.cells.Add(new SaveData.CellData
                {
                    isOccupied = cell.isOcupied,
                    type = cell.type
                });
            }
        }

        

        saveSystem.Save(saveData);
        Debug.Log("Progress Saved");
    }

   public void CreateLevel(int sceneID)
    {
        switch(sceneID)
        {
            case 0:
                break;
            case 1:
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        gridSystem.GetGrid()[11 + x, 2 + y].isOcupied = true;
                        gridSystem.GetGrid()[11 + x, 2 + y].type = GridSystem.CellType.Ocupied;
                    }
                }

                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 1; y++)
                    {
                        gridSystem.GetGrid()[22 + x, 6 + y].isOcupied = true;
                        gridSystem.GetGrid()[22 + x, 6 + y].type = GridSystem.CellType.Ocupied;
                    }
                }

                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 1; y++)
                    {
                        gridSystem.GetGrid()[31 + x, 8 + y].isOcupied = true;
                        gridSystem.GetGrid()[31 + x, 8 + y].type = GridSystem.CellType.Ocupied;
                    }
                }
                break;
            case 2:
                for (int x = 0; x < 7; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        gridSystem.GetGrid()[19 + x, 0 + y].isOcupied = true;
                        gridSystem.GetGrid()[19 + x, 0 + y].type = GridSystem.CellType.Ocupied;
                    }

                }

                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 1; y++)
                    {
                        gridSystem.GetGrid()[30 + x, 7 + y].isOcupied = true;
                        gridSystem.GetGrid()[30 + x, 7 + y].type = GridSystem.CellType.Ocupied;
                    }

                }
                break;
        }
    }

    private int currentLevelIndex { get; set; } = 0;

    public void SpawnLevelObstacles(int index)
    {
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

        Obstacle lastObstacleInstance = null;

        while (!placedSuccessfully && attempts < maxAttempts)
        {
            if (obstaclePrefab.groupObstacle)
            {
                groupInstantiatorManager.InstantiateGroupObstacles(obstaclePrefab, gridSystem);
                placedSuccessfully = true; // Se coloca sin comprobar si bloquea el camino
            }
            else
            {
                if (lastObstacleInstance != null)
                {
                    DestroyObstacle(lastObstacleInstance.gridPos, lastObstacleInstance.size);
                    Destroy(lastObstacleInstance.gameObject);
                }

                lastObstacleInstance = Instantiate(obstaclePrefab, Vector3.zero, Quaternion.identity);
                bool placed = lastObstacleInstance.Init(gridSystem);

                if (placed)
                {
                    placedSuccessfully = true;
                    obstaclesOnCurrentLevel.Add(lastObstacleInstance);
                }
                else
                {
                    Debug.LogWarning($"No se pudo colocar el obstáculo {obstaclePrefab.name}. No hay posiciones válidas.");
                    Destroy(lastObstacleInstance.gameObject);
                    lastObstacleInstance = null;
                }
            }

            attempts++;
        }

        if (!placedSuccessfully)
        {
            Debug.LogError($"No se pudo colocar el obstáculo o grupo {obstaclePrefab.name} después de {maxAttempts} intentos.");
        }
    }

    private IEnumerator RespawnEffectsRoutine()
    {
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(0.5f);

        Time.timeScale = 1;
        player.RespawnPlayer();
        if (globalVolume != null) globalVolume.gameObject.SetActive(false);
        if (respawnText != null) respawnText.gameObject.SetActive(false);
        globalVolumeOliver.gameObject.SetActive(true);
    }
    public void ActivateRespawnEffects()
    {
        if (globalVolume != null) globalVolume.gameObject.SetActive(true);
        if (respawnText != null) respawnText.gameObject.SetActive(true);
        StartCoroutine(RespawnEffectsRoutine());
        RestartObstacles();
    }
    public void GameOver()
    {
        scoreText.text = "Score: " + levelCount;
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    public void FinishLevel()
    {
        
        cameraTweening.DOCameraAnimation(onLevelFinished);
        GameManager.Instance.UpdatePlayerScore(levelCount);
        RestartObstacles();
    }
    private void InitLevel()
    {

        SpawnLevelObstacles(levelCount);
        player.RespawnPlayer();
        levelFinisher.isRespawning = false;
        levelCount++;
    }

    public void RestartGame()
    {
        levelCount = 0;
        destroyManager.rechargeValue = 0;
        player.RespawnPlayer();
        Time.timeScale = 1;
        gameOverCanvas.SetActive(false);
        cameraTweening.DOCameraAnimation(restartLevel);
        gridSystem.DestroyAllObstacles();

        while (obstaclesOnCurrentLevel.Count > 0)
        {
            Destroy(obstaclesOnCurrentLevel[0].gameObject);
            obstaclesOnCurrentLevel.RemoveAt(0);
        }

        CreateLevel(GameManager.Instance.sceneID);

        levelTimer.timeRemaining = 20;

    }

    public void RestartObstacles()
    {
        if (obstaclesOnCurrentLevel.Count == 0) return;
        for (int i = 0; i < obstaclesOnCurrentLevel.Count; i++)
        {
            obstaclesOnCurrentLevel[i].RestartObstacle();
        }
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

                    }
                }
            }

        }
    }

    private void RestartLevel()
    {

    }



}
