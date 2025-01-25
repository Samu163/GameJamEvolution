using UnityEngine;
using DG.Tweening; 

public class CameraTweening : MonoBehaviour
{
    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.rotation;
    }
    public void DOCameraAnimation(LevelManager.OnLevelFinished onLevelFinished)
    {
        DOCameraAnimationUp().OnComplete(() =>
        {
            onLevelFinished();
            DOCameraAnimationDown();
        });
    }
    public Sequence DOCameraAnimationUp()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveZ(transform.position.z - 5, 0.2f));
        sequence.Append(transform.DOMoveY(transform.position.y + 45, 0.4f).SetEase(Ease.OutBack, 0.12f));
        return sequence;
    }
    public void DOCameraAnimationDown()
    {
        Sequence sequence = DOTween.Sequence();        
        sequence.Append(transform.DOMove(new Vector3(transform.position.x, -50, transform.position.z),0.0f));
        sequence.Append(transform.DOMoveY(7, 0.6f).SetEase(Ease.OutBack, 0.12f));
        sequence.Append(transform.DOMoveZ(transform.position.z+5, 0.2f));
    }

    private void Update()
    {
       
    }
}