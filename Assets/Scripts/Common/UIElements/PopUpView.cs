using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public enum PopUpAnimationType
{
    None,
    Fade,
    Scale,
    SlideFromTop,
    SlideFromBottom,
    SlideFromLeft,
    SlideFromRight,
    Shake 
}

public class PopUpView : MonoBehaviour 
{
    [Header("Target Panel")]
    [SerializeField] private RectTransform _targetPanelRectTransform;
    [SerializeField] private CanvasGroup _targetPanelCanvasGroup;
    
    [Header("Show Animation Settings")] 
    [SerializeField] private PopUpAnimationType _showAnimationType = PopUpAnimationType.Fade;
    [SerializeField] private float _showAnimationDuration = 0.3f; 
    [SerializeField] private Ease _showEase = Ease.OutQuad;

    [Header("Hide Animation Settings")] 
    [SerializeField] private PopUpAnimationType _hideAnimationType = PopUpAnimationType.Fade;
    [SerializeField] private float _hideAnimationDuration = 0.3f; 
    [SerializeField] private Ease _hideEase = Ease.InQuad;

    [Header("Slide Effect Settings")]
    [SerializeField] private float _slideOffset = 50f;

    [Header("Shake Effect Settings")] 
    [SerializeField] private float _shakeStrength = 10f; 
    [SerializeField] private int _shakeVibrato = 10;   
    [SerializeField] private float _shakeRandomness = 90f; 
    [SerializeField] private bool _shakeFadeOutEffect = true; 

    [Header("State")]
    [SerializeField] private bool _startHidden = true;
    
    private Vector3 _initialPanelScale;
    private Vector2 _initialPanelAnchoredPosition;
    private Quaternion _initialPanelRotation; 
    
    private Tween _currentTween;
    private bool _isShown = false;

    [Header("Events")]
    public UnityEvent OnShowStart;
    public UnityEvent OnShowComplete;
    public UnityEvent OnHideStart;
    public UnityEvent OnHideComplete;

    void Awake()
    {
        if (_targetPanelRectTransform == null)
        {
            Debug.LogError("PopUpView: Target Panel RectTransform is not assigned!", this);
            enabled = false;
            return;
        }
        
        if (_targetPanelCanvasGroup == null)
        {
            _targetPanelCanvasGroup = _targetPanelRectTransform.GetComponent<CanvasGroup>();
            
            if (_targetPanelCanvasGroup == null)
            {
                _targetPanelCanvasGroup = _targetPanelRectTransform.gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        _initialPanelScale = _targetPanelRectTransform.localScale;
        _initialPanelAnchoredPosition = _targetPanelRectTransform.anchoredPosition;
        _initialPanelRotation = _targetPanelRectTransform.localRotation; 

        if (_startHidden)
        {
            _targetPanelRectTransform.gameObject.SetActive(false); 
            
            if (_targetPanelCanvasGroup != null)
            {
                _targetPanelCanvasGroup.alpha = 0f;
                _targetPanelCanvasGroup.interactable = false;
                _targetPanelCanvasGroup.blocksRaycasts = false;
            }
            
            _isShown = false;
        }
        else 
        {
            _targetPanelRectTransform.gameObject.SetActive(true);
            
            if (_targetPanelCanvasGroup != null)
            {
                _targetPanelCanvasGroup.alpha = 1f;
                _targetPanelCanvasGroup.interactable = true;
                _targetPanelCanvasGroup.blocksRaycasts = true;
            }
            
            _isShown = true;
        }
    }

    public void Show(bool show, bool immediate = false)
    {
        if (_targetPanelRectTransform == null) return; 
        
        if (_isShown == show && _targetPanelRectTransform.gameObject.activeSelf == show)
        {
            if (show && !_targetPanelRectTransform.gameObject.activeSelf) { /* Continue */ }
            else return;
        }

        KillCurrentTween();
        _isShown = show;

        PopUpAnimationType currentAnimType;
        float currentDuration; 
        Ease currentEase;

        if (show)
        {
            currentAnimType = _showAnimationType;
            currentDuration = _showAnimationDuration;
            currentEase = _showEase;
        }
        else
        {
            currentAnimType = _hideAnimationType;
            currentDuration = _hideAnimationDuration;
            currentEase = _hideEase;
        }
        
        if (immediate) 
        {
            currentDuration = 0f;
        }
        
        if (currentDuration <= 0f || currentAnimType == PopUpAnimationType.None) 
        {
            if (show)
            {
                OnShowStart?.Invoke();
                
                _targetPanelRectTransform.localScale = _initialPanelScale;
                _targetPanelRectTransform.anchoredPosition = _initialPanelAnchoredPosition;
                _targetPanelRectTransform.localRotation = _initialPanelRotation; 
                if (_targetPanelCanvasGroup != null) _targetPanelCanvasGroup.alpha = 1f;
                
                _targetPanelRectTransform.gameObject.SetActive(true); 

                if (_targetPanelCanvasGroup != null)
                {
                    _targetPanelCanvasGroup.interactable = true;
                    _targetPanelCanvasGroup.blocksRaycasts = true;
                }
                OnShowComplete?.Invoke();
            }
            else 
            {
                OnHideStart?.Invoke();
                
                if (_targetPanelCanvasGroup != null)
                {
                    if (_hideAnimationType == PopUpAnimationType.Fade || _hideAnimationType == PopUpAnimationType.Shake)
                    {
                         _targetPanelCanvasGroup.alpha = 0f;
                    }
                    _targetPanelCanvasGroup.interactable = false;
                }
                
                _targetPanelRectTransform.gameObject.SetActive(false); 
                
                if (_targetPanelCanvasGroup != null)
                {
                    _targetPanelCanvasGroup.blocksRaycasts = false;
                }
                
                OnHideComplete?.Invoke();
            }
        }
        else 
        {
            if (show) 
            {
                OnShowStart?.Invoke();
                
                _targetPanelRectTransform.localScale = _initialPanelScale; 
                _targetPanelRectTransform.anchoredPosition = _initialPanelAnchoredPosition;
                _targetPanelRectTransform.localRotation = _initialPanelRotation; 

                if (_targetPanelCanvasGroup != null) {
                    _targetPanelCanvasGroup.alpha = (currentAnimType == PopUpAnimationType.Fade) ? 0f : 1f;
                }
                
                _targetPanelRectTransform.gameObject.SetActive(true); 

                if (_targetPanelCanvasGroup != null)
                {
                    _targetPanelCanvasGroup.interactable = true; 
                    _targetPanelCanvasGroup.blocksRaycasts = true;
                }
                
                _currentTween = AnimateShow(currentAnimType, currentDuration, currentEase, () =>
                {
                    if (_targetPanelCanvasGroup != null) _targetPanelCanvasGroup.alpha = 1f; 
                    _targetPanelRectTransform.localScale = _initialPanelScale;
                    _targetPanelRectTransform.anchoredPosition = _initialPanelAnchoredPosition;
                    _targetPanelRectTransform.localRotation = _initialPanelRotation; 
                    OnShowComplete?.Invoke();
                });
            }
            else 
            {
                OnHideStart?.Invoke();
                
                if (_targetPanelCanvasGroup != null)
                {
                    _targetPanelCanvasGroup.interactable = false; 
                }
                
                _currentTween = AnimateHide(currentAnimType, currentDuration, currentEase, () =>
                {
                    if (_hideAnimationType == PopUpAnimationType.Fade && _targetPanelCanvasGroup != null && 
                        !(_currentTween is Sequence)) 
                    {
                        _targetPanelCanvasGroup.alpha = 0f;
                    }
                    
                    _targetPanelRectTransform.gameObject.SetActive(false); 
                    
                    if (_targetPanelCanvasGroup != null)
                    {
                        _targetPanelCanvasGroup.blocksRaycasts = false; 
                    }
                    
                    OnHideComplete?.Invoke();
                });
            }
        }
    }
    
    private Tween AnimateShow(PopUpAnimationType animType, float duration, Ease ease, System.Action onCompleteCallback)
    {
        Tween tween = null;
        if (_targetPanelRectTransform == null) return null;
        
        switch (animType)
        {
            case PopUpAnimationType.Fade:
                if (_targetPanelCanvasGroup != null) tween = _targetPanelCanvasGroup.DOFade(1f, duration);
                else Debug.LogWarning($"PopUpView: Target Panel CanvasGroup not found for Fade.", this);
                break;
            case PopUpAnimationType.Scale:
                tween = _targetPanelRectTransform.DOScale(_initialPanelScale, duration).From(Vector3.zero);
                break;
            case PopUpAnimationType.SlideFromTop:
                tween = _targetPanelRectTransform.DOAnchorPos(_initialPanelAnchoredPosition, duration)
                                     .From(_initialPanelAnchoredPosition + new Vector2(0, _targetPanelRectTransform.rect.height + _slideOffset));
                break;
            case PopUpAnimationType.SlideFromBottom:
                tween = _targetPanelRectTransform.DOAnchorPos(_initialPanelAnchoredPosition, duration)
                                     .From(_initialPanelAnchoredPosition - new Vector2(0, _targetPanelRectTransform.rect.height + _slideOffset));
                break;
            case PopUpAnimationType.SlideFromLeft:
                tween = _targetPanelRectTransform.DOAnchorPos(_initialPanelAnchoredPosition, duration)
                                     .From(_initialPanelAnchoredPosition - new Vector2(_targetPanelRectTransform.rect.width + _slideOffset, 0));
                break;
            case PopUpAnimationType.SlideFromRight:
                tween = _targetPanelRectTransform.DOAnchorPos(_initialPanelAnchoredPosition, duration)
                                     .From(_initialPanelAnchoredPosition + new Vector2(_targetPanelRectTransform.rect.width + _slideOffset, 0));
                break;
            case PopUpAnimationType.Shake:
                tween = _targetPanelRectTransform.DOShakeAnchorPos(
                    duration, _shakeStrength, _shakeVibrato, _shakeRandomness, 
                    snapping: false, fadeOut: _shakeFadeOutEffect 
                );
                break;
        }

        ApplyTweenSettings(tween, ease, onCompleteCallback);
        return tween;
    }

    private Tween AnimateHide(PopUpAnimationType animType, float duration, Ease ease, System.Action onCompleteCallback)
    {
        Tween tween = null;
        if (_targetPanelRectTransform == null) return null;

        switch (animType)
        {
            case PopUpAnimationType.Fade:
                if (_targetPanelCanvasGroup != null) tween = _targetPanelCanvasGroup.DOFade(0f, duration);
                else Debug.LogWarning($"PopUpView: Target Panel CanvasGroup not found for Fade.", this);
                break;
            case PopUpAnimationType.Scale:
                tween = _targetPanelRectTransform.DOScale(Vector3.zero, duration);
                break;
            case PopUpAnimationType.SlideFromTop:
                tween = _targetPanelRectTransform.DOAnchorPos(_initialPanelAnchoredPosition + new Vector2(0, _targetPanelRectTransform.rect.height + _slideOffset), duration);
                break;
            case PopUpAnimationType.SlideFromBottom:
                tween = _targetPanelRectTransform.DOAnchorPos(_initialPanelAnchoredPosition - new Vector2(0, _targetPanelRectTransform.rect.height + _slideOffset), duration);
                break;
            case PopUpAnimationType.SlideFromLeft:
                tween = _targetPanelRectTransform.DOAnchorPos(_initialPanelAnchoredPosition - new Vector2(_targetPanelRectTransform.rect.width + _slideOffset, 0), duration);
                break;
            case PopUpAnimationType.SlideFromRight:
                tween = _targetPanelRectTransform.DOAnchorPos(_initialPanelAnchoredPosition + new Vector2(_targetPanelRectTransform.rect.width + _slideOffset, 0), duration);
                break;
            case PopUpAnimationType.Shake:
                Sequence shakeHideSequence = DOTween.Sequence();
                float shakePartDuration = duration * 0.7f; // Shake
                float fadePartDuration = duration * 0.3f;  // FadeOut

                shakeHideSequence.Append(
                    _targetPanelRectTransform.DOShakeAnchorPos(
                        shakePartDuration, _shakeStrength, _shakeVibrato, _shakeRandomness, 
                        snapping: false, fadeOut: _shakeFadeOutEffect
                    )
                );
                
                if (_targetPanelCanvasGroup != null && fadePartDuration > 0) 
                {
                    shakeHideSequence.Append(
                        _targetPanelCanvasGroup.DOFade(0f, fadePartDuration)
                    );
                } 
                else if (_targetPanelCanvasGroup != null && fadePartDuration <= 0) 
                {
                    shakeHideSequence.AppendCallback(() => {
                        if (_targetPanelCanvasGroup != null) _targetPanelCanvasGroup.alpha = 0f;
                    });
                }
                
                tween = shakeHideSequence;
                break;
        }

        ApplyTweenSettings(tween, ease, onCompleteCallback);
        return tween;
    }

    private void ApplyTweenSettings(Tween tween, Ease ease, System.Action onCompleteCallback)
    {
        if (tween != null)
        {
            tween.SetEase(ease) 
                 .SetUpdate(true) 
                 .OnComplete(() => { onCompleteCallback?.Invoke(); });
        }
        else
        {
            PopUpAnimationType currentAnimType = _isShown ? _showAnimationType : _hideAnimationType;
            
            if (currentAnimType != PopUpAnimationType.None) 
            {
                onCompleteCallback?.Invoke(); 
            }
        }
    }

    private void KillCurrentTween()
    {
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill(false); 
        }
        _currentTween = null;
    }

    void OnDestroy()
    {
        KillCurrentTween();
    }
}