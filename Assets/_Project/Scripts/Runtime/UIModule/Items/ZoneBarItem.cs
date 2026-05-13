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

        private const float HIGHLIGHT_SCALE = 1.4f;
        private const float HIGHLIGHT_DURATION = 0.2f;
        private const float DEFAULT_SCALE = 1f;
        private const float BACKGROUND_ALPHA = 0.35f;

        public void Initialize(ZoneDTO zoneDTO)
        {
            ZoneNumber = zoneDTO.ZoneNumber;
            uiTextZoneNumberValue.text = zoneDTO.ZoneNumber.ToString();
            ApplyZoneTypeVisual(zoneDTO.SpinType);
        }

        public void SetHighlight(bool highlighted)
        {
            float targetScale = highlighted ? HIGHLIGHT_SCALE : DEFAULT_SCALE;
            transform.DOScale(targetScale, HIGHLIGHT_DURATION).SetEase(Ease.OutBack);
        }

        private void ApplyZoneTypeVisual(SpinType spinType)
        {
            Color tier = TierColors.For(spinType);

            if (uiImageZoneBackground != null)
            {
                uiImageZoneBackground.color = new Color(tier.r, tier.g, tier.b, BACKGROUND_ALPHA);
            }

            if (uiImageZoneFrame != null)
            {
                uiImageZoneFrame.color = tier;
            }
        }
    }
}
