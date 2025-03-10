using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum OnClickAnimationType { ScaleDown, FadeOut, Bounce, Shake }
public enum HoverAnimationType { Move, ColorTint, Rotate }

[RequireComponent(typeof(Button)),RequireComponent(typeof(CanvasGroup)), RequireComponent(typeof(Image))]
public class ButtonComponent : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Pulse Animation")]
    [SerializeField] private bool _enablePulseAnimation = false;
    [SerializeField] private float _pulseScale = 1.1f;
    [SerializeField] private float _pulseDuration = 1f;
    [SerializeField] private float _pulseInterval = 1f;

    [Header("Click Animation")]
    [SerializeField] private OnClickAnimationType _onClickAnimation = OnClickAnimationType.ScaleDown;
    [SerializeField] private float _clickAnimationDuration = 0.2f;

    [Header("Hover Animation")]
    [SerializeField] private HoverAnimationType _hoverAnimation = HoverAnimationType.Move;
    [SerializeField] private Vector3 _hoverOffset = new Vector3(20f, 0, 0);
    [SerializeField] private float _hoverDuration = 0.3f;
    [SerializeField] private Color _hoverColor = Color.cyan;
    
    [Header("Delay")]
    [SerializeField] private bool _enableDelay = false;
    [SerializeField] private float _delay = 1.0f;       
    [SerializeField] private float _fadeDuration = 1.0f;

    private Vector3 _originalPosition;
    private Vector3 originalScale;
    private Color _originalColor;
    private RectTransform _rectTransform;
    private Button _button;
    private Image _buttonImage;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;
        originalScale = _rectTransform.localScale;
        _button = GetComponent<Button>();
        _buttonImage = GetComponent<Image>();
        if (_buttonImage != null)
            _originalColor = _buttonImage.color;
        _canvasGroup = GetComponent<CanvasGroup>();

        _button.onClick.AddListener(PlayOnClickAnimation);
    }

    private void Start()
    {
        if (_enablePulseAnimation)
            PulseAnimation();
        
        if(_enableDelay)
            AddingDelay();
    }

    #region Pulse Animation
    
    private void PulseAnimation()
    {
        _rectTransform.DOScale(_pulseScale, _pulseDuration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => Invoke(nameof(PulseAnimation), _pulseInterval));
    }
    
    #endregion

    #region Click Animation
    
    private void PlayOnClickAnimation()
    {
        switch (_onClickAnimation)
        {
            case OnClickAnimationType.ScaleDown:
                _rectTransform.DOScale(0f, _clickAnimationDuration).SetEase(Ease.InBack)
                    .OnComplete(() => _rectTransform.DOScale(originalScale, _clickAnimationDuration).SetEase(Ease.OutBack));
                break;

            case OnClickAnimationType.FadeOut:
                if (_buttonImage)
                {
                    _buttonImage.DOFade(0f, _clickAnimationDuration)
                        .OnComplete(() => _buttonImage.DOFade(1f, _clickAnimationDuration));
                }
                break;

            case OnClickAnimationType.Bounce:
                _rectTransform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), _clickAnimationDuration, 10, 1f);
                break;

            case OnClickAnimationType.Shake:
                _rectTransform.DOShakePosition(_clickAnimationDuration, new Vector3(10, 10, 0));
                break;
        }
    }
    
    #endregion

    #region Hover Animation
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (_hoverAnimation)
        {
            case HoverAnimationType.Move:
                _rectTransform.DOAnchorPos(_originalPosition + (Vector3)_hoverOffset, _hoverDuration)
                    .SetEase(Ease.OutQuad);
                break;

            case HoverAnimationType.ColorTint:
                if (_buttonImage)
                {
                    _buttonImage.DOColor(_hoverColor, _hoverDuration);
                }
                break;

            case HoverAnimationType.Rotate:
                _rectTransform.DORotate(new Vector3(0, 0, 15f), _hoverDuration)
                    .SetEase(Ease.OutQuad);
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch (_hoverAnimation)
        {
            case HoverAnimationType.Move:
                _rectTransform.DOAnchorPos(_originalPosition, _hoverDuration)
                    .SetEase(Ease.OutQuad);
                break;

            case HoverAnimationType.ColorTint:
                if (_buttonImage)
                {
                    _buttonImage.DOColor(_originalColor, _hoverDuration);
                }
                break;

            case HoverAnimationType.Rotate:
                _rectTransform.DORotate(Vector3.zero, _hoverDuration)
                    .SetEase(Ease.OutQuad);
                break;
        }
    }
    
    #endregion

    #region Pointer Down/Up Feedback
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _rectTransform.DOScale(originalScale * 0.95f, 0.1f).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _rectTransform.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad);
    }
    
    #endregion
    
    #region Starting Delay
    
    private void AddingDelay()
    {
        _canvasGroup.alpha = 0;
        
        _canvasGroup.DOFade(1, _fadeDuration)
            .SetDelay(_delay)
            .OnComplete(() =>
            {
                _button.interactable = true;
            });
    }
    
    #endregion
}
