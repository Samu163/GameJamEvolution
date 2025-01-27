using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEditor.Animations;

public class DestroyManager : MonoBehaviour
{

    public GridSystem gridSystem;
    [SerializeField] private int destroyWidth;
    [SerializeField] private int destroyHeight;
    [SerializeField] private bool destroyMode = false;
    public Vector3 mouseScreenPos;
    public Vector3 mouseWorldPos;
    public Vector2Int gridPosition;
    public Vector3 debugGridPosition;
    private Vector2Int destroySize;

    [SerializeField] private Button destroyButton;
    [SerializeField] private Slider rechargeBar;
    [SerializeField] public float rechargeValue = 0;
    [SerializeField] public float maxRecharge;

    public LevelTimer levelTimer;

    [Header("Visual Settings")]
    [SerializeField] private GameObject destroyImageObject;
    [SerializeField] private Canvas canvas;
    [SerializeField] public Animator animator;
    [SerializeField] private GameObject flash;

    [Header("Global Volume Settings")]
    [SerializeField] private Volume globalVolume;
    // Start is called before the first frame update
    void Start()
    {
        destroySize = new Vector2Int(destroyWidth, destroyHeight);
        if (destroyImageObject != null)
        {
            destroyImageObject.SetActive(false);
        }
        flash.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (rechargeValue < maxRecharge)
        {
            destroyButton.interactable = false;
        }
        else if (rechargeValue == maxRecharge)
        {
            destroyButton.interactable = true;
        }

        rechargeBar.value = Mathf.Lerp(rechargeValue / 5, rechargeBar.value, Time.deltaTime);

        mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = 40;

        if (destroyMode)
        {
            if (globalVolume != null) globalVolume.enabled = true;
            Time.timeScale = 0;
            mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            gridPosition = gridSystem.WorldToGridPosition(mouseWorldPos);

            ShowDestroyImage();
            if (Input.GetMouseButtonDown(0))
            {
                DestroySelectedObstacles();
                Time.timeScale = 1;
                levelTimer.timeRemaining -= 10;
                destroyMode = false;
            }
        }
        else
        {

            if (globalVolume != null) globalVolume.enabled = false;
        }

    }
    private void ShowDestroyImage()
    {
        if (destroyImageObject != null && canvas != null)
        {
            debugGridPosition = gridSystem.GridToWorldPosition(gridPosition)
                + new Vector3(gridSystem.cellSize * 0.5f, gridSystem.cellSize * 0.5f, 0);

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(debugGridPosition);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(), screenPosition, canvas.worldCamera, out Vector2 canvasPosition
            );

            RectTransform rectTransform = destroyImageObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = canvasPosition;

            rectTransform.sizeDelta = new Vector2(
                destroySize.x * gridSystem.cellSize * 100,
                destroySize.y * gridSystem.cellSize * 100
            );

            destroyImageObject.SetActive(true);
        }
    }

    private void HideDestroyImage()
    {
        if (destroyImageObject != null)
        {
            destroyImageObject.SetActive(false);


        }
    }
    public void DestroySelectedObstacles()
    {
        if (destroyMode)
        {
            flash.SetActive(true);
            animator.SetTrigger("Flash");

            LevelManager.Instance.DestroyObstacle(gridPosition, destroySize);
            destroyMode = false;
            HideDestroyImage();
            StartCoroutine(WaitAndDisableFlash(0.5f));
        }
    }
    private IEnumerator WaitAndDisableFlash(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        flash.SetActive(false);
    }
    public void ActivateDestroyMode()
    {
        destroyMode = true;
        rechargeValue = 0;
    }

    private void OnDrawGizmos()
    {
        if (destroyMode)
        {
            Gizmos.color = Color.red;
            gridPosition = gridSystem.WorldToGridPosition(mouseWorldPos);
            debugGridPosition = gridSystem.GridToWorldPosition(gridPosition) + new Vector3(gridSystem.cellSize * 0.5f, gridSystem.cellSize * 0.5f, 0);
            debugGridPosition = new Vector3(debugGridPosition.x + 1, debugGridPosition.y + 1, debugGridPosition.z);
            Gizmos.DrawWireCube(debugGridPosition, new Vector3(destroySize.x * gridSystem.cellSize, destroySize.y * gridSystem.cellSize, 1));
        }
    }
   
}
