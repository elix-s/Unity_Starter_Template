using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum OnClickAnimationType { ScaleDown, FadeOut, Bounce, Shake }
public enum HoverAnimationType { Move, ColorTint, Rotate }

[RequireComponent(typeof(Button))]
public class ButtonComponent : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Pulse Animation")]
    [SerializeField] private bool enablePulseAnimation = false;
    [SerializeField] private float pulseScale = 1.1f;
    [SerializeField] private float pulseDuration = 1f;
    [SerializeField] private float pulseInterval = 1f;

    [Header("Click Animation")]
    [SerializeField] private OnClickAnimationType onClickAnimation = OnClickAnimationType.ScaleDown;
    [SerializeField] private float clickAnimationDuration = 0.2f;

    [Header("Hover Animation")]
    [SerializeField] private HoverAnimationType hoverAnimation = HoverAnimationType.Move;
    [SerializeField] private Vector3 hoverOffset = new Vector3(20f, 0, 0);
    [SerializeField] private float hoverDuration = 0.3f;
    [SerializeField] private Color hoverColor = Color.cyan;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Color originalColor;
    private RectTransform rectTransform;
    private Button button;
    private Image buttonImage;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
            originalColor = buttonImage.color;

        button.onClick.AddListener(PlayOnClickAnimation);
    }

    private void Start()
    {
        if (enablePulseAnimation)
            PulseAnimation();
    }

    #region Pulse Animation
    
    private void PulseAnimation()
    {
        rectTransform.DOScale(pulseScale, pulseDuration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => Invoke(nameof(PulseAnimation), pulseInterval));
    }
    
    #endregion

    #region Click Animation
    
    private void PlayOnClickAnimation()
    {
        switch (onClickAnimation)
        {
            case OnClickAnimationType.ScaleDown:
                rectTransform.DOScale(0f, clickAnimationDuration).SetEase(Ease.InBack)
                    .OnComplete(() => rectTransform.DOScale(originalScale, clickAnimationDuration).SetEase(Ease.OutBack));
                break;

            case OnClickAnimationType.FadeOut:
                if (buttonImage)
                {
                    buttonImage.DOFade(0f, clickAnimationDuration)
                        .OnComplete(() => buttonImage.DOFade(1f, clickAnimationDuration));
                }
                break;

            case OnClickAnimationType.Bounce:
                rectTransform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), clickAnimationDuration, 10, 1f);
                break;

            case OnClickAnimationType.Shake:
                rectTransform.DOShakePosition(clickAnimationDuration, new Vector3(10, 10, 0));
                break;
        }
    }
    
    #endregion

    #region Hover Animation
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (hoverAnimation)
        {
            case HoverAnimationType.Move:
                rectTransform.DOAnchorPos(originalPosition + (Vector3)hoverOffset, hoverDuration)
                    .SetEase(Ease.OutQuad);
                break;

            case HoverAnimationType.ColorTint:
                if (buttonImage)
                {
                    buttonImage.DOColor(hoverColor, hoverDuration);
                }
                break;

            case HoverAnimationType.Rotate:
                rectTransform.DORotate(new Vector3(0, 0, 15f), hoverDuration)
                    .SetEase(Ease.OutQuad);
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch (hoverAnimation)
        {
            case HoverAnimationType.Move:
                rectTransform.DOAnchorPos(originalPosition, hoverDuration)
                    .SetEase(Ease.OutQuad);
                break;

            case HoverAnimationType.ColorTint:
                if (buttonImage)
                {
                    buttonImage.DOColor(originalColor, hoverDuration);
                }
                break;

            case HoverAnimationType.Rotate:
                rectTransform.DORotate(Vector3.zero, hoverDuration)
                    .SetEase(Ease.OutQuad);
                break;
        }
    }
    
    #endregion

    #region Pointer Down/Up Feedback
    
    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.DOScale(originalScale * 0.95f, 0.1f).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rectTransform.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad);
    }
    
    #endregion
}
