using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Settings")]
    private Transform player;
    public float rotationSpeed = 2f;

    [Header("Laser Settings")]
    public LineRenderer laserRenderer;
    public float laserMaxDistance = 20f;

    [Header("Collision Settings")]
    public LayerMask collisionLayers;

    [Header("Particle Settings")]
    public ParticleSystem activationParticles;
    private Transform initalLaserTransform;
    private CapsuleCollider laserCollider;
    private Vector3 currentLaserDirection;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentLaserDirection = transform.forward;

        laserRenderer.enabled = false;
        laserCollider = gameObject.AddComponent<CapsuleCollider>();
        laserCollider.isTrigger = true;
        laserCollider.direction = 2;
        laserCollider.enabled = false;
        initalLaserTransform = transform;
        if (activationParticles != null)
        {
            activationParticles.Stop();
        }

        StartCoroutine(LaserCycle());
    }

    private void Update()
    {
        UpdateLaserDirection();
        if (player != null && laserRenderer.enabled)
        {

            FireLaser();
        }
    }

    private void UpdateLaserDirection()
    {
        Vector3 targetDirection = (player.position - transform.position).normalized;
        currentLaserDirection = Vector3.Slerp(currentLaserDirection, targetDirection, rotationSpeed * Time.deltaTime).normalized;
        transform.rotation = Quaternion.LookRotation(currentLaserDirection);
    }

    private void FireLaser()
    {
        if (laserRenderer == null || laserCollider == null) return;

        Vector3 laserStart = transform.position;
        RaycastHit hit;
        Vector3 laserEnd;

        if (Physics.Raycast(laserStart, currentLaserDirection, out hit, laserMaxDistance, collisionLayers))
        {
            laserEnd = hit.point;
            activationParticles.transform.position = hit.point;
            activationParticles.Play();

        }
        else
        {
            laserEnd = laserStart + currentLaserDirection * laserMaxDistance;
            activationParticles.Stop();

        }

        laserEnd.z = 0.5f;
        laserRenderer.SetPosition(0, laserStart);
        laserRenderer.SetPosition(1, laserEnd);

        UpdateLaserCollider(laserStart, laserEnd);
    }

    private void UpdateLaserCollider(Vector3 start, Vector3 end)
    {
        Vector3 center = (start + end) / 2;
        float length = Vector3.Distance(start, end);

        laserCollider.center = transform.InverseTransformPoint(center);
        laserCollider.height = length;
        laserCollider.radius = 0.1f;
        laserCollider.transform.rotation = Quaternion.LookRotation(end - start);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().RespawnPlayer();
        }
    }

    private IEnumerator LaserCycle()
    {
        while (true)
        {
            laserRenderer.enabled = false;
            laserCollider.enabled = false;

            if (activationParticles != null)
            {
                activationParticles.Stop();
                activationParticles.transform.position = initalLaserTransform.position;
            }

            yield return new WaitForSeconds(3f); 

            if (activationParticles != null)
            {
                activationParticles.Play();
            }

            yield return new WaitForSeconds(2f); 

            laserRenderer.enabled = true;
            laserCollider.enabled = true;
            yield return new WaitForSeconds(3f); 
        }
    }
}
