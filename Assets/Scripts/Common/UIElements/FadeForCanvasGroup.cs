using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class LayoutGroupFader : MonoBehaviour
{
    [SerializeField] private float _fadeOutTime = 1.0f;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    
    private async void OnEnable()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.DOFade(0f, _fadeOutTime).SetEase(Ease.Linear);
        int ms = (int)_fadeOutTime * 1000;
        await UniTask.Delay(ms);
        Destroy(gameObject);
    }
}