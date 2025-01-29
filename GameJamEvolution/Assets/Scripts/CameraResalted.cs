using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CameraResalted : MonoBehaviour
{

    private bool isHovered = false;

    public GameObject cameraOutline;
    public Button destroyButton;

    public void ShowOutline()
    {
        if (destroyButton.interactable)
        {
            cameraOutline.SetActive(true);
        }
    }

    public void HideOutline()
    {
        cameraOutline.SetActive(false);
    }

}
