using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformsManager : MonoBehaviour
{
    public List<GameObject> platformsList;
    [SerializeField] private float respawnDelay;
    public Transform groundPlayerCheckBox;

    public void Respawn(GameObject platform)
    {
        StartCoroutine(waitToRespawn(platform));
    }

    IEnumerator waitToRespawn(GameObject platform)
    {
        yield return new WaitForSeconds(respawnDelay);
        platform.SetActive(true);
        platform.transform.position = platform.GetComponent<FallingPlatform>().startPosition;
        platform.GetComponent<Rigidbody>().useGravity = false;
        platform.GetComponent<Rigidbody>().isKinematic = true;
    }
}
