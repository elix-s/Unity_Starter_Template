using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class LoadingScreenView : MonoBehaviour
{
    public void Fade(int fadeOutTime)
    {
        var canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            float time = (float)fadeOutTime / 1000;
            canvasGroup.DOFade(0f, time).SetEase(Ease.Linear);
        }
    }
}
