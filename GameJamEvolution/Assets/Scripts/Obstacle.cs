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
            Destroy(gameObject); // Destruir el GameObject si no puede colocarse
            return false;
        }
    }
    public abstract List<Vector2Int> SpawnPreference(List<Vector2Int> validPositions, GridSystem.Cell[,] grid);

}

