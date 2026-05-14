using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GoldenSpin.CoreModule;

namespace GoldenSpin.UIModule
{
    public class ZoneBarItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text uiTextZoneNumberValue;
        [SerializeField]
        private Image uiImageZoneBackground;
        [SerializeField]
        private Image uiImageZoneFrame;

        public int ZoneNumber { get; private set; }

        private const float BACKGROUND_ALPHA = 0.15f;
        private const float HIGHLIGHT_ALPHA = 0.85f;
        private const float HIGHLIGHT_DURATION = 0.2f;

        private Color tierColor;

        public void Initialize(ZoneDTO zoneDTO)
        {
            ZoneNumber = zoneDTO.ZoneNumber;
            uiTextZoneNumberValue.text = zoneDTO.ZoneNumber.ToString();
            ApplyZoneTypeVisual(zoneDTO.SpinType);
        }

        public void SetHighlight(bool highlighted)
        {
            if (uiImageZoneBackground == null) { return; }

            float targetAlpha = highlighted ? HIGHLIGHT_ALPHA : BACKGROUND_ALPHA;
            Color targetColor = new Color(tierColor.r, tierColor.g, tierColor.b, targetAlpha);
            DOTween.To(
                () => uiImageZoneBackground.color,
                c => uiImageZoneBackground.color = c,
                targetColor,
                HIGHLIGHT_DURATION
            ).SetEase(Ease.OutQuad);
        }

        private void ApplyZoneTypeVisual(SpinType spinType)
        {
            tierColor = TierColors.For(spinType);

            if (uiImageZoneBackground != null)
            {
                uiImageZoneBackground.color = new Color(tierColor.r, tierColor.g, tierColor.b, BACKGROUND_ALPHA);
            }

            if (uiImageZoneFrame != null)
            {
                uiImageZoneFrame.color = tierColor;
            }
        }
    }
}
