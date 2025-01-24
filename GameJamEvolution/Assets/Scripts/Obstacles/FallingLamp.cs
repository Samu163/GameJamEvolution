using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingLamp : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fallDelay = 1.0f;
    [SerializeField] private float respawnDelay = 3.0f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float raycastDistance = 2.0f;
    [SerializeField] private GameObject childColliderObject;

    private Vector3 startPosition;
    private Rigidbody rb;
    private Collider childCollider;

    private void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        if (childColliderObject != null)
        {
            childCollider = childColliderObject.GetComponent<Collider>();
            childCollider.enabled = false;
        }
    }

    private void Update()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, playerLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                StartCoroutine(Fall());
            }
        }
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.isKinematic = false;
        rb.useGravity = true;
        if (childCollider != null)
        {
            childCollider.enabled = true;
        }
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    private void Respawn()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.position = startPosition;
        if (childCollider != null)
        {
            childCollider.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * raycastDistance);
    }
}
