using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimatorManager : MonoBehaviour
{
    public List<GameObject> menuButtons;
    //public List<GameObject> menuIcons;
    public RectTransform gameTitle;


    private void Awake()
    {
        //ResetIconsScale();
    }

    public Sequence StartmenuAnim()
    {
        float delay = AnimateMainMenuButtons();
        AnimateTitle(delay, gameTitle);

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

    public Sequence AnimateTitle(float delay, RectTransform title)
    {
        float originalPositionY = title.anchoredPosition.y;
        Vector2 startPosition = title.anchoredPosition;
        startPosition.y = 500.0f;
        title.anchoredPosition = startPosition;

        title.DOAnchorPosY(originalPositionY - 30.0f, 1.6f)
                 .SetEase(Ease.OutSine)
                 .SetDelay(delay)
                 .OnComplete(() =>
                 {
                     title.DOAnchorPosY(originalPositionY, 0.3f)
                              .SetEase(Ease.InSine);
                 });

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(delay * 2 + 0.6f);
        return sequence;
    }
}
