using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : Obstacle
{

    [SerializeField] private float attackTime;
    [SerializeField] private float attackMaxRadius;
    [SerializeField] private float radiusAugmentation;
    public SphereCollider attackCollider;
    [SerializeField] bool isAttacking;
    private float attackTimeCounter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking)
        {
            attackTimeCounter += Time.deltaTime;
        }
        
        if (attackTimeCounter >= attackTime)
        {
            isAttacking = true;
        }

        if (isAttacking)
        {
           AugmentRadius();

           if (attackCollider.radius >= attackMaxRadius)
           {
                isAttacking = false;
                attackCollider.radius = 0.5f;
                attackTimeCounter = 0;
           }
        }
    }

    void AugmentRadius()
    {
        if (attackCollider.radius < attackMaxRadius)
        {
            attackCollider.radius += radiusAugmentation;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCollider.transform.position, attackCollider.radius);
    }
}
