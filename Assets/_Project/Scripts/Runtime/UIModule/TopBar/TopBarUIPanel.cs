using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.UIModule;

namespace GoldenSpin.UIModule
{
    public class TopBarUIPanel : BasePanel
    {
        [Inject] private IZoneController     zoneController;
        [Inject] private ICurrencyController currencyController;

        [SerializeField] private Image    uiImageZoneDot;
        [SerializeField] private TMP_Text uiTextZoneLabelValue;
        [SerializeField] private TMP_Text uiTextCurrencyAmountValue;

        private static readonly Color ColorNormal = new Color(0.72f, 0.39f, 0.10f);
        private static readonly Color ColorSafe   = new Color(0.60f, 0.60f, 0.93f);
        private static readonly Color ColorSuper  = new Color(1.00f, 0.84f, 0.00f);

        private void Start()
        {
            if (zoneController == null || currencyController == null) return;
            ListenEvents();
            RefreshZone(zoneController.CurrentZoneDTO);
            RefreshCurrency(currencyController.CurrentAmount);
            ShowImmediate();
        }

        private void ListenEvents()
        {
            if (zoneController != null)
                zoneController.OnZoneChanged += RefreshZone;
            if (currencyController != null)
                currencyController.OnCurrencyChanged += RefreshCurrency;
        }

        private void RefreshZone(ZoneDTO zone)
        {
            Color dotColor = zone.Type switch
            {
                ZoneType.Super => ColorSuper,
                ZoneType.Safe  => ColorSafe,
                _              => ColorNormal,
            };

            string label = zone.Type switch
            {
                ZoneType.Super => $"Zone {zone.ZoneNumber} 👑",
                ZoneType.Safe  => $"Safe Zone {zone.ZoneNumber}",
                _              => $"Zone {zone.ZoneNumber}",
            };

            uiImageZoneDot.color    = dotColor;
            uiTextZoneLabelValue.text = label;
        }

        private void RefreshCurrency(int amount)
        {
            uiTextCurrencyAmountValue.text = amount.ToString();
        }

        private void UnsubscribeFromEvents()
        {
            if (zoneController != null)
                zoneController.OnZoneChanged -= RefreshZone;
            if (currencyController != null)
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
                uiImageZoneDot = transform.Find("ui_container_zone_badge/ui_image_zone_dot")?.GetComponent<Image>();

            if (uiTextZoneLabelValue == null)
                uiTextZoneLabelValue = transform.Find("ui_container_zone_badge/ui_text_zone_label_value")?.GetComponent<TMP_Text>();

            if (uiTextCurrencyAmountValue == null)
                uiTextCurrencyAmountValue = transform.Find("ui_container_currency_pill/ui_text_currency_amount_value")?.GetComponent<TMP_Text>();
        }
#endif
    }
}
