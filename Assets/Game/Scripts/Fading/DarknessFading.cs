using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DarknessFading : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        
        canvasGroup.DOFade(0, 2);
    }
}
