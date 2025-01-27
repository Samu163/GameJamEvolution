using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingGround : Obstacle
{
    public int heightMargin = 5;
    public Vector2Int radiusSize;
    public override List<Vector2Int> SpawnPreference(List<Vector2Int> availablePositions, GridSystem.Cell[,] grid, Vector2 size)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        int gridWidth = grid.GetLength(0);
        int gridHeight = grid.GetLength(1);

        foreach (var position in availablePositions)
        {
            bool isValid = true;

            for (int i = 0; i < size.x; i++)
            {
                int checkX = position.x + i;

                if (checkX >= gridWidth || position.y >= gridHeight)
                {
                    isValid = false;
                    break;
                }

                if (grid[checkX, position.y].type != GridSystem.CellType.Empty)
                {
                    isValid = false;
                    break;
                }

                if (position.y == 0 || position.y == 1 || position.y == 2)
                {
                    break;
                }


                for (int j = 0; j < radiusSize.x; j++)
                {
                    for (int k = 0; k < radiusSize.y; k++)
                    {
                        int checkYradius = position.y - 3 + k;
                        int checkXradius = position.x - 2 + j;

                        if (checkXradius < gridWidth && checkYradius < gridHeight && checkXradius >= 0 && checkYradius >= 0)
                        {
                            if (grid[checkXradius, checkYradius].type == GridSystem.CellType.Ground)
                            {
                                isValid = true;
                                break;
                            }
                            else
                            {
                                isValid = false;
                            }
                        }



                    }
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
