using UnityEngine;
using DG.Tweening;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public abstract class BasePanel : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        private Tween fadeTween;
        
        private bool  isVisible;

        public bool IsVisible => isVisible;

        protected virtual void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            SetVisibilityImmediate(false);
        }

        public virtual void Show()
        {
            if (isVisible)
            {
                return;
            }

            isVisible = true;
            gameObject.SetActive(true);
            fadeTween?.Kill();
            canvasGroup.alpha = 0f;
            fadeTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, UIConfig.PANEL_FADE_DURATION).SetEase(Ease.OutQuad);
            OnShow();
        }

        public virtual void Hide()
        {
            if (!isVisible)
            {
                return;
            }

            isVisible = false;
            fadeTween?.Kill();
            fadeTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, UIConfig.PANEL_FADE_DURATION)
                .SetEase(Ease.InQuad)
                .OnComplete(() => gameObject.SetActive(false));
            OnHide();
        }

        public void ShowImmediate()
        {
            isVisible = true;
            fadeTween?.Kill();
            SetVisibilityImmediate(true);
            OnShow();
        }

        public void HideImmediate()
        {
            isVisible = false;
            fadeTween?.Kill();
            SetVisibilityImmediate(false);
            OnHide();
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        private void SetVisibilityImmediate(bool visible)
        {
            gameObject.SetActive(visible);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
            }
        }
    }
}
