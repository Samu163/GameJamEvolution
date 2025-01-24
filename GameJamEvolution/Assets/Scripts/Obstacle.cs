using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Obstacle : MonoBehaviour
{
    public Vector2Int size;

    public bool Init(GridSystem gridSystem)
    {
        
        if (gridSystem.TryPlaceObstacle(size, out Vector2Int position))
        {
            Vector3 worldPosition = gridSystem.GridToWorldPosition(position);
            transform.position = worldPosition;
            return true;
        }
        else
        {
            Debug.LogWarning("No se pudo colocar el obst�culo. El nivel est� lleno.");
            Destroy(gameObject); // Destruir el GameObject si no puede colocarse
            return false;
        }
    }
}

