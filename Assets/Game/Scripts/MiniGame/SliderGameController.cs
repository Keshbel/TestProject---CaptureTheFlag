using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class SliderGameController : MonoBehaviour, IPointerClickHandler
{
    public Action<bool> OnGameResult { get; set; }
    
    [Header("Area")]
    [SerializeField] private RectTransform areaRectTransform;
    [SerializeField] private RectTransform successRectTransform;
    
    [Header("Handle")]
    [SerializeField] private RectTransform handleRectTransform;

    [Header("Settings")] 
    [SerializeField] private float speed = 1f;
    [SerializeField] private float gameDuration = 1.5f;

    private bool _isMovingToRight = true;
    private CancellationTokenSource _handleTokenSource;

    #region Unity Events

    public void OnPointerClick(PointerEventData eventData)
    {
        CheckGameOnTap();
    }

    #endregion

    #region Public Methods
    
    public async void StartMiniGame()
    {
        ShowMiniGame();

        _handleTokenSource = new CancellationTokenSource();

        var duration = gameDuration;
        while (duration > 0)
        {
            MoveHandle();
            duration -= Time.deltaTime;
            
            await Task.Yield();
            
            if (_handleTokenSource.IsCancellationRequested) return;
        }

        OnGameResult?.Invoke(false);

        if (gameObject) HideMiniGame();
    }
    
    public void HideMiniGame()
    {
        gameObject.SetActive(false);
        _handleTokenSource?.Cancel();
    }
    
    #endregion

    #region Private Methods
    
    private void ShowMiniGame()
    {
        ChangeSuccessZonePosition();
        ChangeHandlePosition();
        
        gameObject.SetActive(true);
    }

    private async void CheckGameOnTap()
    {
        var isContain = RectTransformUtility.RectangleContainsScreenPoint(successRectTransform, handleRectTransform.position);
        OnGameResult?.Invoke(isContain);
        _handleTokenSource?.Cancel();
        
        await Task.Delay(500);
        
        HideMiniGame();
    }

    private void MoveHandle()
    {
        var areaRect = areaRectTransform.rect; 
        if (handleRectTransform.anchoredPosition.x <= areaRect.xMin) _isMovingToRight = true;
        else if (handleRectTransform.anchoredPosition.x >= areaRect.xMax) _isMovingToRight = false;
        
        int deltaPosition;
        if (_isMovingToRight) deltaPosition = 1;
        else deltaPosition = -1;
        deltaPosition = (int)(deltaPosition * speed);

        handleRectTransform.anchoredPosition += new Vector2(deltaPosition, 0);
    }

    private void ChangeHandlePosition()
    {
        var areaRect = areaRectTransform.rect;
        var randomPositionX = Random.Range(areaRect.xMin, areaRect.xMax);
        handleRectTransform.anchoredPosition = new Vector2(randomPositionX, 0);
    }
    
    private void ChangeSuccessZonePosition()
    {
        var successZoneHalfWidth = successRectTransform.rect.width / 2;
        
        var areaRect = areaRectTransform.rect;
        var leftLimit = areaRect.xMin + successZoneHalfWidth;
        var rightLimit = areaRect.xMax - successZoneHalfWidth;
        
        var randomPositionX = Random.Range(leftLimit, rightLimit);
        successRectTransform.anchoredPosition = new Vector2(randomPositionX, 0);
    }
    
    #endregion
}
