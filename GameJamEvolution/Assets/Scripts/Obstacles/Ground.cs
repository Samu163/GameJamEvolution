using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : Obstacle
{
    public int heightMargin = 5;
    public override List<Vector2Int> SpawnPreference(List<Vector2Int> valiPositions, GridSystem.Cell[,] grid)
    {
        List<Vector2Int> filteredPositions = new List<Vector2Int>();

        int gridHeight = grid.GetLength(1); 

        foreach (var position in valiPositions)
        {
            if (position.y >= heightMargin && position.y < gridHeight - heightMargin)
            {
                filteredPositions.Add(position);
            }
        }

        return filteredPositions;
    }
}