using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aspiradora : Obstacle
{

    [SerializeField] private Vector3 aspirationForce;
    [SerializeField] private float aspirationTime;
    [SerializeField] private float aspirationTimeCounter;
    [SerializeField] private bool isAspiring = false;
   

    // Update is called once per frame
    void Update()
    {

        if (!isAspiring)
        {
            aspirationTimeCounter += Time.deltaTime;
        }
        else if (isAspiring)
        {
            aspirationTimeCounter -= Time.deltaTime / 2;
        }

        if (aspirationTimeCounter >= aspirationTime)
        {
            isAspiring = true;
        }
        else if (aspirationTimeCounter <= 0)
        {
            isAspiring = false;
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && isAspiring)
        {
            other.gameObject.GetComponent<Rigidbody>().velocity -= aspirationForce;
        }
    }
}
