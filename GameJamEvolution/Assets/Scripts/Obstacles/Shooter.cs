using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Settings")]
    private Transform player;
    public float rotationSpeed = 2f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float fireRate = 1f;

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

        transform.rotation = Quaternion.LookRotation(currentDirection);
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);
        projectile.transform.rotation = rotationToPlayer * Quaternion.Euler(0, 40, 0);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = directionToPlayer * projectileSpeed;
            rb.useGravity = false;
        }
    }

}
