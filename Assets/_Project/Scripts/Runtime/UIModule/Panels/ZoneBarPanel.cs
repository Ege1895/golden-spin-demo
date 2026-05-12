using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public class ZoneBarPanel : BasePanel
    {
        [Inject]
        private IZoneController    zoneController;

        [Inject]
        private ZoneBarItemFactory zoneBarItemFactory;

        [SerializeField]
        private Transform zonesContainer;

        [SerializeField]
        private RectTransform scrollContent;

        [SerializeField]
        private TMP_Text uiTextZoneNumberValue;

        [SerializeField]
        private TMP_Text uiTextZoneTypeValue;

        private readonly List<ZoneBarItem> visibleItems = new List<ZoneBarItem>();

        private void Start()
        {
            zoneBarItemFactory.SetParent(zonesContainer);
            ListenEvents();
            BuildZoneBar();
            RefreshZoneTexts(zoneController.CurrentZoneDTO);
            ShowImmediate();
        }

        private void ListenEvents()
        {
            zoneController.OnZoneChanged += HandleZoneChanged;
        }

        private void BuildZoneBar()
        {
            zoneBarItemFactory.DestroyAll();
            visibleItems.Clear();

            int currentZone = zoneController.CurrentZone;
            int startZone   = Mathf.Max(1, currentZone - 1);
            int endZone     = startZone + UIConfig.ZONE_BAR_VISIBLE_COUNT - 1;

            for (int i = startZone; i <= endZone; i++)
            {
                ZoneDTO    zoneDTO = zoneController.GetZoneDTO(i);
                ZoneBarItem item   = zoneBarItemFactory.Create(zoneDTO);
                visibleItems.Add(item);

                if (i == currentZone)
                {
                    item.SetHighlight(true);
                }
            }
        }

        private void HandleZoneChanged(ZoneDTO zoneDTO)
        {
            BuildZoneBar();
            RefreshZoneTexts(zoneDTO);
        }

        private void RefreshZoneTexts(ZoneDTO zoneDTO)
        {
            if (uiTextZoneNumberValue != null)
                uiTextZoneNumberValue.text = zoneDTO.ZoneNumber.ToString();
            if (uiTextZoneTypeValue != null)
                uiTextZoneTypeValue.text = zoneDTO.Type.ToString().ToUpper();
        }

        private void UnsubscribeFromEvents()
        {
            zoneController.OnZoneChanged -= HandleZoneChanged;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}
