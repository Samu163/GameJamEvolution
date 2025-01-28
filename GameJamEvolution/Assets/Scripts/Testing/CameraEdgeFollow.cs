using UnityEngine;

public class CameraEdgeFollow : MonoBehaviour
{
    [SerializeField] private Vector2 movementLimits = new Vector2(10f, 5f); 
    [SerializeField] private float smoothness = 0.1f;
    private Vector3 initialPosition; 

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Vector2 normalizedMousePosition = new Vector2(
            (mousePosition.x / screenWidth) * 2 - 1,
            (mousePosition.y / screenHeight) * 2 - 1
        );

        Vector3 targetPosition = new Vector3(
            Mathf.Clamp(initialPosition.x + normalizedMousePosition.x * movementLimits.x, initialPosition.x - movementLimits.x, initialPosition.x + movementLimits.x),
            Mathf.Clamp(initialPosition.y + normalizedMousePosition.y * movementLimits.y, initialPosition.y - movementLimits.y, initialPosition.y + movementLimits.y),
            initialPosition.z 
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness);
    }
}
