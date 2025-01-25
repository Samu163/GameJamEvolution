using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinisher : MonoBehaviour
{
    public DestroyManager destroyManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.InitNewLevel();
            if (destroyManager.rechargeValue < destroyManager.maxRecharge)
            {
                destroyManager.rechargeValue++;
            }
            
        }
    }
}
