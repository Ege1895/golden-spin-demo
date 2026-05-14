using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using GoldenSpin.CoreModule;

namespace GoldenSpin.UIModule
{
    public class TopBarUIPanel : BasePanel
    {
        [Inject]
        private IZoneController zoneController;
        [Inject]
        private ICurrencyController currencyController;

        [SerializeField]
        private Image uiImageZoneDot;

        [SerializeField]
        private TMP_Text uiTextZoneLabelValue;
        [SerializeField]
        private TMP_Text uiTextCurrencyAmountValue;

        private const string ZONE_LABEL_SUPER_PREFIX = "SUPER Zone ";
        private const string ZONE_LABEL_SAFE_PREFIX = "Safe Zone ";
        private const string ZONE_LABEL_PREFIX = "Zone ";

#if UNITY_EDITOR
        private const string PATH_IMAGE_ZONE_DOT = "ui_container_zone_badge/ui_image_zone_dot";
        private const string PATH_TEXT_ZONE_LABEL = "ui_container_zone_badge/ui_text_zone_label_value";
        private const string PATH_TEXT_CURRENCY_AMOUNT = "ui_container_currency_pill/ui_text_currency_amount_value";
#endif

        protected override void Awake()
        {
            base.Awake();
            ListenEvents();
        }

        private void Start()
        {
            RefreshZone(zoneController.CurrentZoneDTO);
            RefreshCurrency(currencyController.CurrentAmount);
            ShowImmediate();
        }

        private void ListenEvents()
        {
            zoneController.OnZoneChanged += RefreshZone;
            currencyController.OnCurrencyChanged += RefreshCurrency;
        }

        private void RefreshZone(ZoneDTO zone)
        {
            if (zone == null)
            {
                return;
            }

            Color dotColor = TierColors.For(zone.Type);
            string label = zone.Type switch
            {
                ZoneType.Super => $"{ZONE_LABEL_SUPER_PREFIX}{zone.ZoneNumber}",
                ZoneType.Safe => $"{ZONE_LABEL_SAFE_PREFIX}{zone.ZoneNumber}",
                _ => $"{ZONE_LABEL_PREFIX}{zone.ZoneNumber}",
            };

            uiImageZoneDot.color = dotColor;
            uiTextZoneLabelValue.text = label;
        }

        private void RefreshCurrency(int amount)
        {
            uiTextCurrencyAmountValue.text = amount.ToString();
        }

        private void UnsubscribeFromEvents()
        {
            zoneController.OnZoneChanged -= RefreshZone;
            currencyController.OnCurrencyChanged -= RefreshCurrency;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiImageZoneDot == null)
            {
                uiImageZoneDot = transform.Find(PATH_IMAGE_ZONE_DOT)?.GetComponent<Image>();
            }

            if (uiTextZoneLabelValue == null)
            {
                uiTextZoneLabelValue = transform.Find(PATH_TEXT_ZONE_LABEL)?.GetComponent<TMP_Text>();
            }

            if (uiTextCurrencyAmountValue == null)
            {
                uiTextCurrencyAmountValue = transform.Find(PATH_TEXT_CURRENCY_AMOUNT)?.GetComponent<TMP_Text>();
            }
        }
#endif
    }
}
