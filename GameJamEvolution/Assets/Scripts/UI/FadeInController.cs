using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 

public class FadeInController : MonoBehaviour
{
    public Image fadeImage;
    public static FadeInController instance;
    public float fadeDuration = 2.0f;

    private void Awake()
    {
        instance = this;
    }

    public void StartFadeOut(System.Action onComplete = null)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(0f, fadeDuration).OnComplete(() => {
            onComplete?.Invoke(); 
        });
    }

    public void StartFadeIn(System.Action onComplete = null)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(1f, fadeDuration).OnComplete(() => {
            onComplete?.Invoke();
        });
    }

    public void ResetAlpha(int alpha, bool active)
    {
        fadeImage.DOFade(alpha, 0);
        fadeImage.gameObject.SetActive(active);
    }
}
