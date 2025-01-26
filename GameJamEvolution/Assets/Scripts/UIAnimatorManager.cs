using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimatorManager : MonoBehaviour
{
    public List<GameObject> menuButtons;
    public List<GameObject> menuIcons;
    public RectTransform gameTitle;

    private void Awake()
    {
        ResetIconsScale();
    }

    public Sequence StartmenuAnim()
    {
        float delay = AnimateMainMenuButtons();
        AnimateTitle(delay);
        AnimateMenuIcons(delay * 2);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(delay * 2 +0.6f);
        return sequence;
    }
    private float AnimateMainMenuButtons()
    {
        List<float> originalPositionsX = new List<float>();

        for (int i = 0; i < menuButtons.Count; i++)
        {
            RectTransform rectTransform = menuButtons[i].GetComponent<RectTransform>();
            originalPositionsX.Add(rectTransform.anchoredPosition.x);

            Vector2 startPosition = rectTransform.anchoredPosition;
            startPosition.x = -500.0f;
            rectTransform.anchoredPosition = startPosition;
            float delay = 0.3f * i; 
            float overshootDistance = 30.0f; 

            Sequence buttonSequence = DOTween.Sequence();
            buttonSequence
                .Append(rectTransform.DOAnchorPosX(originalPositionsX[i] + overshootDistance, 0.6f) 
                                      .SetEase(Ease.OutSine))
                .Append(rectTransform.DOAnchorPosX(originalPositionsX[i], 0.3f) 
                                      .SetEase(Ease.InSine))
                .SetDelay(delay); 
        }
        return 0.3f * menuButtons.Count;
    }

    private void AnimateTitle(float delay)
    {
        float originalPositionY = gameTitle.anchoredPosition.y;
        Vector2 startPosition = gameTitle.anchoredPosition;
        startPosition.y = 500.0f;
        gameTitle.anchoredPosition = startPosition;

        gameTitle.DOAnchorPosY(originalPositionY - 30.0f, 1.6f)
                 .SetEase(Ease.OutSine)
                 .SetDelay(delay)
                 .OnComplete(() =>
                 {
                     gameTitle.DOAnchorPosY(originalPositionY, 0.3f)
                              .SetEase(Ease.InSine);
                 });
    }

    private void AnimateMenuIcons(float baseDelay)
    {
        for (int i = 0; i < menuIcons.Count; i++)
        {
            RectTransform rectTransform = menuIcons[i].GetComponent<RectTransform>();
            float delay = baseDelay + 0.3f * i;

            Sequence iconSequence = DOTween.Sequence();
            iconSequence
                .Append(rectTransform.DOScale(Vector3.one, 0.6f) 
                                      .SetEase(Ease.OutBack)) 
                .SetDelay(delay);
        }
    }


    private void ResetIconsScale()
    {
        for (int i = 0; i < menuIcons.Count; i++)
        {
            RectTransform rectTransform = menuIcons[i].GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.zero;
        }
    }

}
