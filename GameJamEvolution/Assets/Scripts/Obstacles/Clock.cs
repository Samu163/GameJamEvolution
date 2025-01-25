using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : Obstacle
{

    [SerializeField] private float attackTime;
    [SerializeField] private float attackMaxRadius;
    [SerializeField] private float radiusAugmentation;
    public SphereCollider attackCollider;
    [SerializeField] bool isAttacking;
    private float attackTimeCounter;
    public Vector2Int radiusSize;
    public Vector2Int radiusNoClock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking)
        {
            attackTimeCounter += Time.deltaTime;
        }
        
        if (attackTimeCounter >= attackTime)
        {
            isAttacking = true;
        }

        if (isAttacking)
        {
            attackCollider.enabled = true;
            AugmentRadius();

           if (attackCollider.radius >= attackMaxRadius)
           {
                isAttacking = false;
                attackCollider.radius = 0f;
                attackCollider.enabled = false;
                attackTimeCounter = 0;
           }
        }
    }

    void AugmentRadius()
    {
        if (attackCollider.radius < attackMaxRadius)
        {
            attackCollider.radius += radiusAugmentation;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCollider.transform.position, attackCollider.radius);
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
                else if (checkX == gridWidth - 1 || checkX == gridWidth - 2 || checkX == gridWidth - 3)
                {
                    isValid = false;
                    break;
                }
                else if (checkX == 0 || checkX == 1 || checkX == 2)
                {
                    isValid = false;
                    break;
                }
                

                for (int j = 0; j < radiusNoClock.x; j++)
                {
                    for (int k = 0; k < radiusNoClock.y; k++)
                    {
                        int checkYradius = position.y - 2 + k;
                        int checkXradius = position.x - 2 + j;

                        if (checkXradius < gridWidth && checkYradius < gridHeight && checkXradius >= 0 && checkYradius >= 0)
                        {
                            if (grid[checkXradius, checkYradius].type == GridSystem.CellType.Ocupied)
                            {
                                isValid = false;
                                break;
                            }
                           
                        }
                    }
                }

                
                if (position.y == 0 || position.y == 1)
                {
                    isValid = false;
                    break;
                }
                else if (position.y == 2 || position.y == 3 || position.y == 4)
                {
                    continue;
                }


                for (int j = 0; j < radiusSize.x; j++)
                {
                    for (int k = 0; k < radiusSize.y; k++)
                    {
                        int checkYradius = position.y + k;
                        int checkXradius = position.x + j;

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

