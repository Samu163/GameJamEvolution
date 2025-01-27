using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Settings")]
    private Transform player;
    public float rotationSpeed = 2f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab; // Prefab del proyectil
    public float projectileSpeed = 10f; // Velocidad del proyectil
    public float fireRate = 1f; // Intervalo de disparo en segundos

    private Vector3 currentDirection;
    private float nextFireTime;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentDirection = transform.forward;
    }

    private void Update()
    {
        if (player != null)
        {
            UpdateDirection();

            if (Time.time >= nextFireTime)
            {
                FireProjectile();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private void UpdateDirection()
    {
        Vector3 targetDirection = (player.position - transform.position).normalized;
        currentDirection = Vector3.Slerp(currentDirection, targetDirection, rotationSpeed * Time.deltaTime).normalized;
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;

        // Calcular la dirección precisa hacia el jugador
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Instanciar el proyectil
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(directionToPlayer));

        // Configurar el Rigidbody del proyectil
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = directionToPlayer * projectileSpeed;
            rb.useGravity = false; // Asegurar que la gravedad está desactivada
        }
    }

}
