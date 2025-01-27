using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 5f; // Tiempo de vida del proyectil antes de destruirse automáticamente

    private void Start()
    {
        // Destruir el proyectil después de un tiempo, por seguridad
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto impactado tiene la capa Ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject); // Destruir el proyectil
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            
            Destroy(gameObject); // Destruir el proyectil
        }
    }
}
