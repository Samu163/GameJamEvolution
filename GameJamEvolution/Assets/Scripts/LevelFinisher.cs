using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinisher : MonoBehaviour
{
    public DestroyManager destroyManager;
    public LevelTimer levelTimer;
    public bool isRespawning;
    private Collider collider;
    public float Timeadded = 10;
    public float gainRecharge = 8;
    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRespawning)
        {
            if (!isRespawning)
            {
                LevelManager.Instance.FinishLevel();    
            }
            isRespawning = true;
            levelTimer.timeRemaining += Timeadded;
            if (destroyManager.rechargeValue < destroyManager.maxRecharge)
            {
                destroyManager.rechargeValue = destroyManager.rechargeValue + gainRecharge;
            }
            
        }
    }

    public void EnableCollider(bool condition)
    {
        //collider.enabled = condition;
    }
}
