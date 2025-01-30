using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoManager : MonoBehaviour
{
    public RectTransform targetImage; 
    public RectTransform texto; 
    public float duration = 0.5f; 

    void Start()
    {
        StartScaleAnimation(targetImage,0.0f, 3.0f);
        StartScaleAnimation(texto,1.0f, 1.0f);
    }

    public void StartScaleAnimation(RectTransform taget, float delay ,float value)
    {
        if (targetImage == null) return;

        taget.localScale = Vector3.zero;

        taget.DOScale(value, duration + delay).SetEase(Ease.OutBack);
    }



}
