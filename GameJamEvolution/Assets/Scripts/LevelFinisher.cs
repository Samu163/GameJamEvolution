using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinisher : MonoBehaviour
{
    public DestroyManager destroyManager;
    public LevelTimer levelTimer;
    public bool isRespawning;
    private Collider collider;

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
            levelTimer.timeRemaining += 20;
            if (destroyManager.rechargeValue < destroyManager.maxRecharge)
            {
                destroyManager.rechargeValue++;
            }
            
        }
    }

    public void EnableCollider(bool condition)
    {
        //collider.enabled = condition;
    }
}
