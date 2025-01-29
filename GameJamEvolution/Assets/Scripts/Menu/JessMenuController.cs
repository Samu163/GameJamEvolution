using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class JessMenuController : MonoBehaviour
{
    public float headWeight = 1.0f;
    public float bodyWeight = 0.5f;
    public float distanceToCamera = 10.0f;

    private Vector3 mouseWorldPos; 
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPos.x,
                mouseScreenPos.y,
                distanceToCamera
            ));
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (anim)
        {
            anim.SetLookAtPosition(mouseWorldPos);
            anim.SetLookAtWeight(1, bodyWeight, headWeight);
        }
    }
}
