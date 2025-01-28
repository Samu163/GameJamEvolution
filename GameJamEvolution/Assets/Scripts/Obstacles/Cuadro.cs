using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuadro : Obstacle
{
    public Laser laser;
    public override void RestartObstacle()
    {
        if (isLaser && laser != null)
        {
            laser.RestartLaser();
        }
        
    }
    public override List<Vector2Int> SpawnPreference(List<Vector2Int> availablePositions, GridSystem.Cell[,] grid, Vector2 size)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        int gridWidth = grid.GetLength(0);
        int gridHeight = grid.GetLength(1);
        int minX = 15;
        int maxX = 24;
        int occupiedPaintingCount = 0;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].type == GridSystem.CellType.OcupiedPainting)
                {
                    occupiedPaintingCount++;
                }
            }
        }

        if (occupiedPaintingCount >= 9)
        {
            return possiblePositions;
        }

        foreach (var position in availablePositions)
        {
            bool isValid = true;

            if (position.y < 8)
            {
                isValid = false;
                continue;
            }

            if (position.x < minX || position.x > maxX)
            {
                isValid = false;
                continue;
            }
            for (int offsetX = -5; offsetX <= 5; offsetX++)
            {
                for (int offsetY = -5; offsetY <= 5; offsetY++)
                {
                    int checkX = position.x + offsetX;
                    int checkY = position.y + offsetY;

                    if (checkX >= 0 && checkX < gridWidth && checkY >= 0 && checkY < gridHeight)
                    {
                        if (grid[checkX, checkY].type == GridSystem.CellType.OcupiedPainting)
                        {
                            isValid = false;
                            break;
                        }
                    }
                }

                if (!isValid)
                {
                    break;
                }
            }

            for (int offsetX = 0; offsetX < size.x; offsetX++)
            {
                int currentX = position.x + offsetX;

                if (currentX >= gridWidth || position.y >= gridHeight)
                {
                    isValid = false;
                    break;
                }

                if (grid[currentX, position.y].type == GridSystem.CellType.OcupiedPainting)
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                possiblePositions.Add(position);
            }
        }

        return possiblePositions;
    }
}
