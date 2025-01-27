using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Closet : Obstacle
{
    [Header("Settings")]
    [SerializeField] private float activateDuration = 2.0f;
    [SerializeField] private float delay = 2.0f; 
    [SerializeField] private GameObject childColliderObject;

    private Collider childCollider;

    private void Start()
    {
        if (childColliderObject != null)
        {
            childCollider = childColliderObject.GetComponent<Collider>();
            childCollider.enabled = false;
        }
        else
        {
            Debug.LogError("No se asignó el objeto del Collider hijo en el inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ActivateColliderTemporarily());
        }
    }

    private IEnumerator ActivateColliderTemporarily()
    {
        yield return new WaitForSeconds(delay);
        if (childCollider != null)
        {
            childCollider.enabled = true;
        }

        yield return new WaitForSeconds(activateDuration);

        if (childCollider != null)
        {
            childCollider.enabled = false;
        }
    }

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

                if (position.y > 0 && grid[checkX, position.y - 1].type != GridSystem.CellType.Ground)
                {
                    isValid = false;
                    break;
                }

                int leftX = position.x - 1;
                int rightX = position.x + 1;

                if ((leftX >= 0 && grid[leftX, position.y].type != GridSystem.CellType.Empty) ||
                    (rightX < gridWidth && grid[rightX, position.y].type != GridSystem.CellType.Empty))
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
