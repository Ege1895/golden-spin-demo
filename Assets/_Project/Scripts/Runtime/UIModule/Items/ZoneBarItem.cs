using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GoldenSpin.CoreModule;
using Zenject;

namespace GoldenSpin.UIModule
{
    public class ZoneBarItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text ui_text_zone_number_value;
        [SerializeField]
        private Image ui_image_zone_background;
        [SerializeField]
        private Image ui_image_zone_icon;

        public int ZoneNumber { get; private set; }
        
        private const float HIGHLIGHT_SCALE    = 1.2f;
        private const float HIGHLIGHT_DURATION = 0.2f;

        public void Initialize(ZoneDTO zoneDTO)
        {
            ZoneNumber = zoneDTO.ZoneNumber;
            ui_text_zone_number_value.text = zoneDTO.ZoneNumber.ToString();
            ApplyZoneTypeVisual(zoneDTO.Type);
        }

        public void SetHighlight(bool highlighted)
        {
            float targetScale = highlighted ? HIGHLIGHT_SCALE : 1f;
            transform.DOScale(targetScale, HIGHLIGHT_DURATION).SetEase(Ease.OutBack);
        }

        private void ApplyZoneTypeVisual(ZoneType zoneType)
        {
            switch (zoneType)
            {
                case ZoneType.Super:
                    ui_image_zone_background.color = Color.yellow;
                    ui_image_zone_icon.gameObject.SetActive(true);
                    break;
                case ZoneType.Safe:
                    ui_image_zone_background.color = Color.gray;
                    ui_image_zone_icon.gameObject.SetActive(false);
                    break;
                default:
                    ui_image_zone_background.color = Color.white;
                    ui_image_zone_icon.gameObject.SetActive(false);
                    break;
            }
        }

        public class Factory : PlaceholderFactory<ZoneDTO, ZoneBarItem> { }
    }
}
