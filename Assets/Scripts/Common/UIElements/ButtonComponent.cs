using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool showPulseAnimation = false;
    private Vector3 _originalPosition;
    
    private void Awake()
    {
        _originalPosition = transform.position; 
        GetComponent<Button>().onClick.AddListener(()=>ReduceButton());
    }

    private void Start()
    {
        if (showPulseAnimation) PulseAnimation();
    }
    
    private void PulseAnimation()
    {
        transform.DOScale(1.1f, 1f) 
            .SetLoops(2, LoopType.Yoyo) 
            .SetEase(Ease.InOutSine) 
            .OnComplete(() => Invoke(nameof(PulseAnimation), 1.0f)); 
    }
    
    public void ReduceButton()
    {
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOMove(_originalPosition + new Vector3(20f, 0, 0), 0.3f) 
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOMove(_originalPosition, 0.3f).SetEase(Ease.OutQuad);
    }
}
