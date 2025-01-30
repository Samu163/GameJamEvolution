using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelController : MonoBehaviour
{
    public int levelIndex;

    public float hoverScale; 
    public float animationDuration; 
    public float rotationAngle; 
    private Vector3 originalScale; 
    private bool isHovered = false;

    //public UnityEngine.Events.UnityEvent onClick; 

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void OnMouseEnter()
    {
        if (!isHovered)
        {
            isHovered = true;
            transform.DOScale(originalScale * hoverScale, animationDuration).SetEase(Ease.OutBack);
            transform.DORotate(new Vector3(0, 0, rotationAngle), animationDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnMouseExit()
    {
        if (isHovered)
        {
            isHovered = false;
            transform.DOKill();
            transform.DOScale(originalScale, animationDuration).SetEase(Ease.OutBack);
            transform.rotation = Quaternion.identity; 
        }
    }

    private void OnMouseDown()
    {
        FadeInController.instance.StartFadeIn(() => {
            switch (levelIndex)
            {
                case 1:
                    GameManager.Instance.sceneID = 0;
                    GameManager.Instance.LoadSceneRequest("GameScene");
                    break;
                case 2:
                    GameManager.Instance.sceneID = 1;
                    GameManager.Instance.LoadSceneRequest("GameScene2Nueva");
                    break;
                case 3:
                    GameManager.Instance.sceneID = 2;
                    GameManager.Instance.LoadSceneRequest("GameScene3Nueva");
                    break;
                default:
                    Debug.LogWarning("Invalid level index");
                    break;
            }
        });
       
    }
}
