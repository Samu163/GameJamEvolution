using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Closet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float activateDuration = 2.0f;
    [SerializeField] private float delay = 2.0f; 
    [SerializeField] private GameObject childColliderObject;

    private Collider childCollider;

    private void Start()
    {
        if (childColliderObject != null)
        {
            childCollider = childColliderObject.GetComponent<Collider>();
            childCollider.enabled = false;
        }
        else
        {
            Debug.LogError("No se asignó el objeto del Collider hijo en el inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ActivateColliderTemporarily());
        }
    }

    private IEnumerator ActivateColliderTemporarily()
    {
        yield return new WaitForSeconds(delay);
        if (childCollider != null)
        {
            childCollider.enabled = true;
        }

        yield return new WaitForSeconds(activateDuration);

        if (childCollider != null)
        {
            childCollider.enabled = false;
        }
    }
}
