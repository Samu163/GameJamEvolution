using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : Obstacle
{

    [SerializeField] private float fallDelay;
    [SerializeField] private float destroyDelay;
    public Vector3 startPosition;
    public Vector2Int radiusSize;
    public Transform groundPlayerCheckBox;
    private bool touchPlayer = false;
    [SerializeField] private LayerMask playerLayer;


    private void Start()
    {
        startPosition = transform.position;
        
    }

    private void Update()
    {
        touchPlayer = Physics.CheckBox(transform.position, new Vector3(1f, 1f, 1f), Quaternion.identity, playerLayer);

        if (touchPlayer)
        {
            StartCoroutine(Fall());
        }

        touchPlayer = false;
    }

    

    IEnumerator Fall()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(fallDelay);
        // Set the platform's rigidbody to kinematic
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        // Wait for 2 seconds
        yield return new WaitForSeconds(destroyDelay);
        // Destroy the platform
        SendMessageUpwards("Respawn", gameObject);
        gameObject.SetActive(false);
       
    }

    public override List<Vector2Int> SpawnPreference(List<Vector2Int> availablePositions, GridSystem.Cell[,] grid)
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

                if (position.y == 0 || position.y == 1)
                {
                    isValid = false;
                    break;
                }
                else if (position.y == gridHeight - 1 || position.y == gridHeight - 2)
                {
                    isValid = false;
                    break;
                }
                else if (grid[checkX, position.y + 1].type == GridSystem.CellType.Ground || grid[checkX, position.y + 2].type == GridSystem.CellType.Ground)
                {
                    isValid = false;
                    break;
                }
                else if (position.y == 2)
                {
                    continue;
                }
                else if (grid[checkX, position.y - 1].type == GridSystem.CellType.Ground || grid[checkX, position.y - 2].type == GridSystem.CellType.Ground)
                {
                    isValid = false;
                    break;
                }
                else if (grid[checkX, position.y - 3].type == GridSystem.CellType.Ground)
                {
                    isValid = true;
                    break;
                }

                for (int j = 0; j < radiusSize.x; j++)
                {
                    for (int k = 0; k < radiusSize.y; k++)
                    {
                        int checkYradius = position.y - 2 + k;
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
