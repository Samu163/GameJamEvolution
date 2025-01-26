using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinisher : MonoBehaviour
{
    public DestroyManager destroyManager;
    private Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.FinishLevel();
            if (destroyManager.rechargeValue < destroyManager.maxRecharge)
            {
                destroyManager.rechargeValue++;
            }
            
        }
    }

    public void EnableCollider(bool condition)
    {
        collider.enabled = condition;
    }
}
