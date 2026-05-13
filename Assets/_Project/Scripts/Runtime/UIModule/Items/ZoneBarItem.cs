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

        public int ZoneNumber { get; private set; }

        private const float HIGHLIGHT_SCALE = 1.4f;
        private const float HIGHLIGHT_DURATION = 0.2f;
        private const float DEFAULT_SCALE = 1f;

        public void Initialize(ZoneDTO zoneDTO)
        {
            ZoneNumber = zoneDTO.ZoneNumber;
            uiTextZoneNumberValue.text = zoneDTO.ZoneNumber.ToString();
            ApplyZoneTypeVisual(zoneDTO.Type);
        }

        public void SetHighlight(bool highlighted)
        {
            float targetScale = highlighted ? HIGHLIGHT_SCALE : DEFAULT_SCALE;
            transform.DOScale(targetScale, HIGHLIGHT_DURATION).SetEase(Ease.OutBack);
        }

        private void ApplyZoneTypeVisual(ZoneType zoneType)
        {
            switch (zoneType)
            {
                case ZoneType.Super:
                    uiImageZoneBackground.color = Color.yellow;
                    break;
                case ZoneType.Safe:
                    uiImageZoneBackground.color = Color.gray;
                    break;
                default:
                    uiImageZoneBackground.color = Color.white;
                    break;
            }
        }
    }
}
