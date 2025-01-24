using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 1f;
    private bool[,] grid;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        grid = new bool[gridWidth, gridHeight];
    }

    // Unity invoca este método cada vez que se cambian valores en el Inspector
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

                if (grid[checkX, checkY]) return false;
            }
        }
        return true;
    }

    public void PlaceObstacle(Vector2Int position, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                grid[position.x + x, position.y + y] = true;
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

    public bool TryPlaceObstacle(Vector2Int size, out Vector2Int placedPosition)
    {
        List<Vector2Int> validPositions = GetValidPositions(size);

        if (validPositions.Count > 0)
        {
            placedPosition = validPositions[Random.Range(0, validPositions.Count)];
            PlaceObstacle(placedPosition, size);
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
                grid[position.x + x, position.y + y] = false;
            }
        }
    }

    // Método para dibujar la cuadrícula y las celdas ocupadas/libres
    private void OnDrawGizmos()
    {
        if (grid == null) grid = new bool[gridWidth, gridHeight];

        // Calcular el origen de la cuadrícula (esquina inferior izquierda)
        Vector3 gridOrigin = transform.position; // Usar directamente la posición del GridSystem

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Calcular la posición de la celda
                Vector3 cellPosition = gridOrigin + new Vector3(x * cellSize, y * cellSize, 0);

                // Elegir el color: verde si está libre, rojo si está ocupada
                Gizmos.color = grid[x, y] ? Color.red : Color.green;

                // Dibujar la celda como un cuadro
                Gizmos.DrawWireCube(cellPosition + Vector3.one * (cellSize * 0.5f), Vector3.one * cellSize);

                // Si está ocupada, rellenar el cuadro
                if (grid[x, y])
                {
                    Gizmos.color = new Color(1, 0, 0, 0.3f); // Rojo semitransparente
                    Gizmos.DrawCube(cellPosition + Vector3.one * (cellSize * 0.5f), Vector3.one * cellSize);
                }
            }
        }
    }
}

