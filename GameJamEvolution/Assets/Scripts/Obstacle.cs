using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Obstacle : MonoBehaviour
{
    public Vector2Int size;
    public Vector2Int gridPos;
    public int id;

    public bool Init(GridSystem gridSystem)
    {
        
        if (gridSystem.TryPlaceObstacle(size, out Vector2Int position))
        {
            Vector3 worldPosition = gridSystem.GridToWorldPosition(position);
            transform.position = worldPosition;
            gridPos = gridSystem.WorldToGridPosition(worldPosition);
            return true;
        }
        else
        {
            Debug.LogWarning("No se pudo colocar el obstáculo. El nivel está lleno.");
            Destroy(gameObject); // Destruir el GameObject si no puede colocarse
            return false;
        }
    }
}

