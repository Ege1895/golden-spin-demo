using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using GoldenSpin.UIModule;

namespace GoldenSpin.GameModule
{
    public class SplashUIPanel : BasePanel
    {
        [SerializeField]
        private Image uiImageLogo;

        [SerializeField]
        private TMP_Text uiTextLoadingValue;

        private const string LOADING_TEXT = "LOADING...";
        private const float LOGO_PUNCH_SCALE = 0.15f;
        private const float LOGO_PUNCH_DURATION = 0.6f;
        private const int LOGO_PUNCH_VIBRATO = 4;
        private const float LOGO_PUNCH_ELASTICITY = 0.6f;
        private const float SPLASH_VISIBLE_DURATION = 1.2f;
        private const float SPLASH_TEXT_DOT_INTERVAL = 0.25f;

#if UNITY_EDITOR
        private const string PATH_IMAGE_LOGO = "ui_image_logo";
        private const string PATH_TEXT_LOADING = "ui_text_loading_value";
#endif

        private Tween loadingTween;

        protected override void Awake()
        {
            base.Awake();
            ShowImmediate();
        }

        private void Start()
        {
            PlayLogoAnimation();
            StartLoadingTextLoop();
            DOVirtual.DelayedCall(SPLASH_VISIBLE_DURATION, HideSplash);
        }

        private void PlayLogoAnimation()
        {
            if (uiImageLogo == null)
            {
                return;
            }

            uiImageLogo.transform.localScale = Vector3.one;
            uiImageLogo.transform.DOPunchScale(
                Vector3.one * LOGO_PUNCH_SCALE,
                LOGO_PUNCH_DURATION,
                LOGO_PUNCH_VIBRATO,
                LOGO_PUNCH_ELASTICITY
            );
        }

        private void StartLoadingTextLoop()
        {
            if (uiTextLoadingValue == null)
            {
                return;
            }

            uiTextLoadingValue.text = LOADING_TEXT;
            loadingTween = uiTextLoadingValue
                .DOFade(0.3f, SPLASH_TEXT_DOT_INTERVAL)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void HideSplash()
        {
            loadingTween?.Kill();
            Hide();
        }

        private void OnDestroy()
        {
            loadingTween?.Kill();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiImageLogo == null)
            {
                uiImageLogo = transform.Find(PATH_IMAGE_LOGO)?.GetComponent<Image>();
            }

            if (uiTextLoadingValue == null)
            {
                uiTextLoadingValue = transform.Find(PATH_TEXT_LOADING)?.GetComponent<TMP_Text>();
            }
        }
#endif
    }
}
