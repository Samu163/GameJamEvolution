using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class MenuButtonsAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int levelIndex;

    public float hoverScale;
    public float animationDuration;
    private Vector3 originalScale;
    private bool isHovered = false;
    private void Start()
    {
        originalScale = gameObject.transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovered)
        {
            isHovered = true;
            gameObject.transform.DOScale(originalScale * hoverScale, animationDuration).SetEase(Ease.OutBack);



        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHovered)
        {
            isHovered = false;
            gameObject.transform.DOKill();
            gameObject.transform.DOScale(originalScale, animationDuration).SetEase(Ease.OutBack);
            gameObject.transform.rotation = Quaternion.identity;
            
        }
    }
}
