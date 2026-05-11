using System.Collections;
using UnityEngine;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public abstract class BasePanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        private bool      isVisible;
        private Coroutine fadeCoroutine;

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

            StopFade();
            fadeCoroutine = StartCoroutine(FadeTo(1f, UIConfig.PANEL_FADE_DURATION));
            OnShow();
        }

        public virtual void Hide()
        {
            if (!isVisible)
            {
                return;
            }

            isVisible = false;

            StopFade();
            fadeCoroutine = StartCoroutine(FadeOutAndDeactivate(UIConfig.PANEL_FADE_DURATION));
            OnHide();
        }

        public void ShowImmediate()
        {
            isVisible = true;
            StopFade();
            SetVisibilityImmediate(true);
            OnShow();
        }

        public void HideImmediate()
        {
            isVisible = false;
            StopFade();
            SetVisibilityImmediate(false);
            OnHide();
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        private void StopFade()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
        }

        private IEnumerator FadeTo(float targetAlpha, float duration)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsed    = 0f;

            while (elapsed < duration)
            {
                elapsed          += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            fadeCoroutine     = null;
        }

        private IEnumerator FadeOutAndDeactivate(float duration)
        {
            yield return FadeTo(0f, duration);
            gameObject.SetActive(false);
            fadeCoroutine = null;
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
