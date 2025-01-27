using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface ISaveSystem
{
    void Save(SaveData saveData);
    SaveData Load();
}

[System.Serializable]
public class SaveData
{
    public int levelCount = 0;
    public float timeRemaining = 20;
    public float destroyCharge = 0;
    public List<CellData> cells = new List<CellData>();
    public List<ObstacleData> obstaclesInGrid = new List<ObstacleData>();

    [System.Serializable]
    public class CellData
    {
        public bool isOccupied;
        public GridSystem.CellType type;
    }

    [System.Serializable]
    public class ObstacleData
    {
        public Vector2Int size;
        public Vector2Int gridPos;
        public GridSystem.CellType cellType;
        public Obstacle.ObstacleType type;
        public bool isFallingPlatform;
        public bool isLaser;
    }
}

public class SaveSystem : ISaveSystem
{
    private string filePath;

    public SaveSystem()
    {
        filePath = Application.persistentDataPath + "/setting.json";
    }

    public void Save(SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(filePath, json);
        Debug.Log("Data saved to: " + filePath);
    }

    public SaveData Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            return saveData;
        }
        else
        {
            Debug.LogWarning("Save file not found. Returning default SaveData.");
            return new SaveData();
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.LogWarning("Save file not found.");
        }
    }

    public bool CheckSaveFile()
    {
        return File.Exists(filePath);
    }
}
