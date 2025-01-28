using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CameraLevelSelectorController : MonoBehaviour
{
    public Camera mainCamera; 
    public GameObject frame;
    public CanvasGroup fadeOverlay; 

    [Header("Camera Animation Settings")]
    public float startZ = -10f;
    public float endZ = -5f;
    public float animationDuration = 2f;
    public float bounceStrength = 1.2f; 

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 1; 
        }

        StartCoroutine(AnimateCameraWithFade());
    }

    private IEnumerator AnimateCameraWithFade()
    {
        mainCamera.transform.DOMoveZ(endZ, animationDuration)
            .From(startZ) 
            .SetEase(Ease.OutBounce);

        if (fadeOverlay != null)
        {
            fadeOverlay.DOFade(0, fadeDuration-1.0f); 
            yield return new WaitForSeconds(fadeDuration); 
            fadeOverlay.gameObject.SetActive(false); 
        }
    }
}
