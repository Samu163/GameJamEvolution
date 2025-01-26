using UnityEngine;
using UnityEngine.EventSystems;
using Linework.Common.Attributes;

public class Outline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField][RenderingLayerMask] private int outlineLayer;
    [SerializeField] private Activate activate = Activate.OnHover;

    private Renderer[] renderers;
    private uint originalLayer;
    private bool isOutlineActive;

    private enum Activate
    {
        OnHover,
        OnClick
    }

    private void Start()
    {
        renderers = TryGetComponent<Renderer>(out var meshRenderer)
            ? new[] { meshRenderer }
            : GetComponentsInChildren<Renderer>();
        originalLayer = renderers[0].renderingLayerMask;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (activate != Activate.OnHover) return;
        SetOutline(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (activate != Activate.OnHover) return;
        SetOutline(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (activate != Activate.OnClick) return;
        isOutlineActive = !isOutlineActive;
        SetOutline(isOutlineActive);
    }

    private void SetOutline(bool enable)
    {
        foreach (var rend in renderers)
        {
            rend.renderingLayerMask = enable
            ? originalLayer | 1u << (int)Mathf.Log(outlineLayer, 2)
            : originalLayer;
        }
    }
}