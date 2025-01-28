using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Obstacle
{
    public override void RestartObstacle()
    {
        Debug.Log("No hace nada");
    }
    public override List<Vector2Int> SpawnPreference(List<Vector2Int> availablePositions, GridSystem.Cell[,] grid,Vector2 groupSize)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        int gridWidth = grid.GetLength(0);
        int gridHeight = grid.GetLength(1);

        foreach (var position in availablePositions)
        {
            bool isValid = true;

            for (int i = 0; i < groupSize.x; i++)
            {
                int checkX = position.x + i;

                if (checkX >= gridWidth || position.y >= gridHeight)
                {
                    isValid = false;
                    break;
                }

                if (position.y > 0 && grid[checkX, position.y - 1].type == GridSystem.CellType.FallingGround)
                {
                    isValid = false;
                    break;
                }

                if (position.y == 0)
                {
                    continue; 
                }
                else if (grid[checkX, position.y - 1].type != GridSystem.CellType.Ground)
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
