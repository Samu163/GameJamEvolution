using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Obstacle : MonoBehaviour
{
    public Vector2Int size;
    public Vector2Int gridPos;
    public GridSystem.CellType cellType;
    public bool groupObstacle;
    public bool isFallingPlatform;
    public bool isLaser;

    public enum ObstacleType
    {
        Ground,
        SlidingGround,
        FallingPlatform,
        Lamp,
        Closet,
        Cuadro,
        Aspiradora,
        Clock,
        Spike
    }
    public ObstacleType type;

    protected void PlayObstacleSound(string soundName, float volumeMultiplier = 1f)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySpecificEffect(type.ToString(), soundName, volumeMultiplier);
        }
    }

    protected void PlayObstacleSoundGroup(float volumeMultiplier = 1f)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayEffect(type.ToString(), volumeMultiplier);
        }
    }

    protected void StopObstacleSound(string soundName)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.StopEffect(type.ToString(), soundName);
        }
    }

    public bool Init(GridSystem gridSystem)
    {
        if (gridSystem.TryPlaceObstacle(size,this, out Vector2Int position))
        {
            Vector3 worldPosition = gridSystem.GridToWorldPosition(position);
            transform.position = worldPosition;
            gridPos = gridSystem.WorldToGridPosition(worldPosition);
            return true;
        }
        else
        {
            Debug.LogWarning("No se pudo colocar el obstáculo. El nivel está lleno.");
            Destroy(gameObject);
            return false;
        }
    }

    public abstract List<Vector2Int> SpawnPreference(List<Vector2Int> validPositions, GridSystem.Cell[,] grid, Vector2 groupSize);

    public abstract void RestartObstacle();
}

