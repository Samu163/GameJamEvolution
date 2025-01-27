using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingLamp : Obstacle
{
    [Header("Settings")]
    [SerializeField] private float fallDelay = 1.0f;
    [SerializeField] private float respawnDelay = 3.0f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float raycastDistance = 2.0f;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private GameObject childColliderObject;

    private Vector3 startPosition;
    private Rigidbody rb;
    private Collider childCollider;

    private void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        if (childColliderObject != null)
        {
            childCollider = childColliderObject.GetComponent<Collider>();
            childCollider.enabled = false;
        }
    }

    private void Update()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        Vector3 origin = raycastOrigin != null ? raycastOrigin.position : transform.position;
        Ray ray = new Ray(origin, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, playerLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                StartCoroutine(Fall());
            }
        }
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.isKinematic = false;
        rb.useGravity = true;
        if (childCollider != null)
        {
            childCollider.enabled = true;
        }
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    private void Respawn()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.position = startPosition;
        if (childCollider != null)
        {
            childCollider.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = raycastOrigin != null ? raycastOrigin.position : transform.position;
        Gizmos.DrawLine(origin, origin + Vector3.down * raycastDistance);
    }

    public override List<Vector2Int> SpawnPreference(List<Vector2Int> availablePositions, GridSystem.Cell[,] grid, Vector2 size)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        int gridWidth = grid.GetLength(0);
        int gridHeight = grid.GetLength(1);

        foreach (var position in availablePositions)
        {
            bool isValid = true;

            if (position.y < 4)
            {
                isValid = false;
            }

            if (!isValid)
            {
                continue;
            }

            for (int i = 1; i <= 4; i++)
            {
                int checkY = position.y - i;

                for (int offsetX = 0; offsetX < size.x; offsetX++)
                {
                    int checkX = position.x + offsetX;

                    if (checkX >= gridWidth || checkY < 0) continue;

                    if (grid[checkX, checkY].type == GridSystem.CellType.Ground)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (!isValid)
                {
                    break;
                }
            }

            if (!isValid)
            {
                continue;
            }

            for (int offsetX = 0; offsetX < size.x; offsetX++)
            {
                int checkX = position.x + offsetX;

                if (checkX >= gridWidth) continue;

                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[checkX, y].type == GridSystem.CellType.OcupiedLamp)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (!isValid)
                {
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
