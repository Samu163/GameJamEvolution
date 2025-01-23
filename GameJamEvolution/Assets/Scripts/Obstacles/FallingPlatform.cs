using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{

    [SerializeField] private float fallDelay;
    [SerializeField] private float destroyDelay;
    public Vector3 startPosition;
  

    private void Start()
    {
        startPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the object that collided with the platform is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Start the coroutine that will make the platform fall
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(fallDelay);
        // Set the platform's rigidbody to kinematic
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        // Wait for 2 seconds
        yield return new WaitForSeconds(destroyDelay);
        // Destroy the platform
        SendMessageUpwards("Respawn", gameObject);
        gameObject.SetActive(false);
       
    }

    
}
