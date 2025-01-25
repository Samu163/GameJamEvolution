using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GroupInstantiatorManager : MonoBehaviour
{
    
    public GameObject fallingPlatformsManager;

    public void InstantiateGroupObstacles(Obstacle obstaclePrefab, GridSystem gridSystem)
    {
        int sizeX = Random.Range(1, 4);

        Vector2Int size = new Vector2Int(sizeX, 1);

        if (gridSystem.TryPlaceObstacle(size, obstaclePrefab, out Vector2Int position))
        {
            for (int i = 0; i < size.x; i++)
            {
                Obstacle obstacle = Instantiate(obstaclePrefab);
                Vector2Int placedPosition = new Vector2Int(position.x + i, position.y);
                Vector3 worldPosition = gridSystem.GridToWorldPosition(placedPosition);
                obstacle.transform.position = worldPosition;
                obstacle.gridPos = gridSystem.WorldToGridPosition(worldPosition);
                LevelManager.Instance.obstaclesOnCurrentLevel.Add(obstacle);

                if (obstacle.isFallingPlatform)
                {
                    obstacle.transform.SetParent(fallingPlatformsManager.transform);
                    fallingPlatformsManager.GetComponent<FallingPlatformsManager>().platformsList.Add(obstacle.gameObject);
                    obstacle.GetComponent<FallingPlatform>().groundPlayerCheckBox = fallingPlatformsManager.GetComponent<FallingPlatformsManager>().groundPlayerCheckBox;
                }
            }
        }
        else
        {
            Debug.LogWarning("No se pudo colocar el obstáculo. El nivel está lleno.");
        }
    }

}
