using UnityEngine;
using DG.Tweening;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public abstract class BasePanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        private bool isVisible;

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
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, UIConfig.PANEL_FADE_DURATION).SetEase(Ease.OutQuad);
            OnShow();
        }

        public virtual void Hide()
        {
            if (!isVisible)
            {
                return;
            }

            isVisible = false;
            canvasGroup.DOFade(0f, UIConfig.PANEL_FADE_DURATION)
                .SetEase(Ease.InQuad)
                .OnComplete(() => gameObject.SetActive(false));
            OnHide();
        }

        public void ShowImmediate()
        {
            isVisible = true;
            SetVisibilityImmediate(true);
            OnShow();
        }

        public void HideImmediate()
        {
            isVisible = false;
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
