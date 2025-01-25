using System.Collections.Generic;
using UnityEngine;

public class PlayerPathChecker
{
    private GridSystem gridSystem;

    public PlayerPathChecker(GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
    }

    public bool IsPathClear(Vector2Int start, Vector2Int end)
    {
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        PriorityQueue<Vector2Int> openSet = new PriorityQueue<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet.Dequeue();

            if (current == end)
            {
                return true;
            }

            closedSet.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor)) continue;

                var cell = gridSystem.GetGrid()[neighbor.x, neighbor.y];
                if (cell.isOcupied)
                {
                    closedSet.Add(neighbor);
                    continue;
                }

                int tentativeGScore = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, end);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
        }

        return false;
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0), 
            new Vector2Int(-1, 0), 
            new Vector2Int(0, 1), 
            new Vector2Int(0, -1) 
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbor = position + direction;

            if (neighbor.x >= 0 && neighbor.x < gridSystem.gridWidth && neighbor.y >= 0 && neighbor.y < gridSystem.gridHeight)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}