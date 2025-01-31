using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampMove : MonoBehaviour
{

    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the tag 'Player'.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player) // Check if the colliding object is the player
        {
            player.transform.SetParent(transform); // Attach player to platform
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            player.transform.SetParent(null); // Detach player from platform
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject == player) // Check if the colliding object is the player
    //    {
    //        player.transform.SetParent(transform); // Attach player to platform
    //    }
    //  Debug.Log("Player entered the trigger");
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject == player)
    //    {
    //        player.transform.SetParent(null); // Detach player from platform
    //    }
    //}
}
