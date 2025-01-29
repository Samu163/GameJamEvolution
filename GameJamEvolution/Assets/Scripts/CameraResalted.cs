using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CameraResalted : MonoBehaviour
{

    private bool isHovered = false;

    public GameObject cameraOutlineSelected;
    public GameObject cameraOutlineNoSelected;
    public Button destroyButton;
    private bool isHovering = false;

    private void Update()
    {
        if (destroyButton.interactable && !isHovering)
        {
            cameraOutlineNoSelected.SetActive(true);
            cameraOutlineSelected.SetActive(false);
        }
        else if (!destroyButton.interactable)
        {
            cameraOutlineNoSelected.SetActive(false);
        }
    }

    public void ShowOutline()
    {

        if (destroyButton.interactable)
        {
            isHovering = true;
            cameraOutlineSelected.SetActive(true);
            cameraOutlineNoSelected.SetActive(false);
        }
    }

    public void HideOutline()
    {
        cameraOutlineSelected.SetActive(false);
        if (destroyButton.interactable)
        {
            cameraOutlineNoSelected.SetActive(true);
        }
    }

}
