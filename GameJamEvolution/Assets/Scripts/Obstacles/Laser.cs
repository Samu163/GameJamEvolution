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

    private CapsuleCollider laserCollider;
    private Vector3 currentLaserDirection;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentLaserDirection = transform.forward;
        laserRenderer.enabled = true;
        laserCollider = gameObject.AddComponent<CapsuleCollider>();
        laserCollider.isTrigger = true;
        laserCollider.direction = 2;
    }

    private void Update()
    {
        if (player != null)
        {
            UpdateLaserDirection();

            FireLaser();
        }
    }

    private void UpdateLaserDirection()
    {
        Vector3 targetDirection = (player.position - transform.position).normalized;

        currentLaserDirection = Vector3.Slerp(currentLaserDirection, targetDirection, rotationSpeed * Time.deltaTime).normalized;
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
        }
        else
        {
            laserEnd = laserStart + currentLaserDirection * laserMaxDistance;
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
            //other.GetComponent<PlayerController>().RespawnPlayer();
        }
    }

}
