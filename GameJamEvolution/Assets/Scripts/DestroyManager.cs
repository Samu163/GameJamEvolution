using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DestroyManager : MonoBehaviour
{

    public GridSystem gridSystem;
    [SerializeField] private int destroyWidth;
    [SerializeField] private int destroyHeight;
    [SerializeField] private bool destroyMode = false;
    public Vector3 mouseScreenPos;
    public Vector3 mouseWorldPos;
    public Vector2Int gridPosition;
    public Vector3 debugGridPosition;
    private Vector2Int destroySize;

    // Start is called before the first frame update
    void Start()
    {
        destroySize = new Vector2Int(destroyWidth, destroyHeight);
    }

    // Update is called once per frame
    void Update()
    {
        

        mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = 55;

        if (destroyMode)
        {
            
            mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            gridPosition = gridSystem.WorldToGridPosition(mouseWorldPos);
            
            if (Input.GetMouseButtonDown(0))
            {
                DestroySelectedObstacles();
                destroyMode = false;
            }
        }

    }

    public void DestroySelectedObstacles()
    {
        if (destroyMode)
        {
           LevelManager.Instance.DestroyObstacle(gridPosition, destroySize);
        }
    }

    public void ActivateDestroyMode()
    {
        destroyMode = true;
    }

    private void OnDrawGizmos()
    {
        if (destroyMode)
        {
            Gizmos.color = Color.red;
            gridPosition = gridSystem.WorldToGridPosition(mouseWorldPos);
            debugGridPosition = gridSystem.GridToWorldPosition(gridPosition) + new Vector3(gridSystem.cellSize * 0.5f, gridSystem.cellSize * 0.5f, 0);
            debugGridPosition = new Vector3(debugGridPosition.x + 1, debugGridPosition.y + 1, debugGridPosition.z);
            Gizmos.DrawWireCube(debugGridPosition, new Vector3(destroySize.x * gridSystem.cellSize, destroySize.y * gridSystem.cellSize, 1));
        }
    }
}
