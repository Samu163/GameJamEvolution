using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float parallaxFactor = 0.5f; // Factor de paralaje para este objeto
    private Vector3 initialPosition;
    private Transform cameraTransform;

    private void Start()
    {
        // Guardar posici�n inicial y referencia a la c�mara principal
        initialPosition = transform.position;
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // Calcular desplazamiento relativo sin afectar la posici�n en z
        Vector3 cameraDisplacement = cameraTransform.position - initialPosition;
        transform.position = new Vector3(
            initialPosition.x + cameraDisplacement.x * parallaxFactor,
            initialPosition.y + cameraDisplacement.y * parallaxFactor,
            initialPosition.z + cameraDisplacement.z * parallaxFactor
        );
    }
}