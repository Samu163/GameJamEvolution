using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UISoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private bool playHoverSound = true;
    private bool playClickSound = true;
    private UISoundController soundController;

    public void Initialize(UISoundController controller, bool hover, bool click)
    {
        soundController = controller;
        playHoverSound = hover;
        playClickSound = click;
        Debug.Log($"Initialized UISoundHandler on {gameObject.name}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playHoverSound && soundController != null)
        {
            Debug.Log($"Hover detected on {gameObject.name}");
            soundController.PlayHoverSound();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playClickSound && soundController != null)
        {
            Debug.Log($"Click detected on {gameObject.name}");
            soundController.PlayClickSound();
        }
    }
} 