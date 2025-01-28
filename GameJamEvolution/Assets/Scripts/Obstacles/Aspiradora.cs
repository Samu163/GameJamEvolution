using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aspiradora : Obstacle
{

    [SerializeField] private Vector3 aspirationForce;
    [SerializeField] private float aspirationTime;
    [SerializeField] private float aspirationTimeCounter;
    [SerializeField] private bool isAspiring = false;
    [SerializeField] private int freeSpaceSize;
    public GameObject particles;
   

    // Update is called once per frame
    void Update()
    {

        if (!isAspiring)
        {
            particles.SetActive(false);
            aspirationTimeCounter += Time.deltaTime;
        }
        else if (isAspiring)
        {
            particles.SetActive(true);
            aspirationTimeCounter -= Time.deltaTime;
        }

        if (aspirationTimeCounter >= aspirationTime)
        {
            isAspiring = true;
        }
        else if (aspirationTimeCounter <= 0)
        {
            isAspiring = false;
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && isAspiring)
        {
            aspirationForce = other.gameObject.transform.position - transform.position;
            aspirationForce.Normalize();
            aspirationForce = new Vector3(aspirationForce.x, aspirationForce.y, 0);
            aspirationForce *= 0.3f;
            other.gameObject.GetComponent<Rigidbody>().velocity -= aspirationForce;
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

                for (int j = 0; j <= freeSpaceSize; j++)
                {
                    int checkSpaceX = position.x + 1 + j;

                    if (checkSpaceX < gridWidth)
                    {
                        if (grid[checkSpaceX, position.y].type == GridSystem.CellType.Ground)
                        {
                            isValid = false;
                            break;
                        }
                    }
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
