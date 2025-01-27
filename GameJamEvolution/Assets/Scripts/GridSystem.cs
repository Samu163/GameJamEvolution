using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridSystem : MonoBehaviour
{
    [System.Serializable]
    public enum CellType { Empty, Ground, Ocupied, OcupiedLamp, OcupiedPainting }

    [System.Serializable]
    public struct Cell
    {
        public bool isOcupied;
        public CellType type;
    }

    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 1f;
    private Cell[,] grid;
    public Cell[,] GetGrid() => grid;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        grid = new Cell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = new Cell { isOcupied = false, type = CellType.Empty };
            }
        }
    }

    public void SetGrid(Cell[,] newGrid)
    {
        grid = newGrid;
    }

    private void OnValidate()
    {
        gridWidth = Mathf.Max(1, gridWidth);
        gridHeight = Mathf.Max(1, gridHeight);

        Init();
    }

    public bool CanPlaceObstacle(Vector2Int position, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int checkX = position.x + x;
                int checkY = position.y + y;

                if (checkX < 0 || checkX >= gridWidth || checkY < 0 || checkY >= gridHeight)
                {
                    return false;
                }

                if (grid[checkX, checkY].isOcupied) return false;
            }
        }
        return true;
    }

    public void PlaceObstacle(Vector2Int position, Vector2Int size, CellType type)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                grid[position.x + x, position.y + y] = new Cell { isOcupied = true, type = type };
            }
        }
    }

    public List<Vector2Int> GetValidPositions(Vector2Int size)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (CanPlaceObstacle(position, size))
                {
                    validPositions.Add(position);
                }
            }
        }

        return validPositions;
    }

    public bool TryPlaceObstacle(Vector2Int size, Obstacle obstacle, out Vector2Int placedPosition)
    {
        List<Vector2Int> validPositions = GetValidPositions(size);

        validPositions = obstacle.SpawnPreference(validPositions, grid);

        if (validPositions.Count > 0)
        {
            placedPosition = validPositions[Random.Range(0, validPositions.Count)];
            PlaceObstacle(placedPosition, size, obstacle.cellType);
            return true;
        }

        placedPosition = Vector2Int.zero;
        return false;
    }

    // Método para convertir coordenadas de cuadrícula a coordenadas de mundo
    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * cellSize, gridPosition.y * cellSize, 0) + transform.position;
    }

    // Método para convertir coordenadas de mundo a coordenadas de cuadrícula
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - transform.position;
        int x = Mathf.FloorToInt(relativePosition.x / cellSize);
        int y = Mathf.FloorToInt(relativePosition.y / cellSize);

        // Limitar las coordenadas a los límites de la cuadrícula
        x = Mathf.Clamp(x, 0, gridWidth - 1);
        y = Mathf.Clamp(y, 0, gridHeight - 1);

        return new Vector2Int(x, y);
    }

    public void DestroyObstacle(Vector2Int position, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                grid[position.x + x, position.y + y].isOcupied = false;
                grid[position.x + x, position.y + y].type = CellType.Empty;
            }
        }
    }

    public void DestroyAllObstacles()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y].isOcupied = false;
                grid[x, y].type = CellType.Empty;
            }
        }
    }

    public List<Vector2Int> GetPositionsWithObstacles(Vector2Int size)
    {
        List<Vector2Int> positionsWithObstacles = new List<Vector2Int>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (!CanPlaceObstacle(position, size))
                {
                    positionsWithObstacles.Add(position);
                }
            }
        }

        return positionsWithObstacles;
    }




    private void OnDrawGizmos()
    {
        if (grid == null) grid = new Cell[gridWidth, gridHeight];

        Vector3 gridOrigin = transform.position;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 cellPosition = gridOrigin + new Vector3(x * cellSize, y * cellSize, 0);

                if (grid[x, y].isOcupied)
                {
                    Gizmos.color = grid[x, y].type == CellType.Ground ? Color.red : Color.blue;
                    Gizmos.DrawWireCube(cellPosition + Vector3.one * (cellSize * 0.5f), Vector3.one * cellSize);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(cellPosition + Vector3.one * (cellSize * 0.5f), Vector3.one * cellSize);
                }
            }
        }
    }
}
