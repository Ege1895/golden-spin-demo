using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.UIModule;

namespace GoldenSpin.ZoneModule
{
    public class ZoneBarUIPanel : BasePanel
    {
        [Inject]
        private IZoneController zoneController;

        [Inject]
        private ZoneBarItemFactory zoneBarItemFactory;

        [SerializeField]
        private Transform zonesContainer;

        [SerializeField]
        private RectTransform scrollContent;

        [SerializeField]
        private TMP_Text uiTextZoneCurrentValue;

        private readonly List<ZoneBarItem> visibleItems = new List<ZoneBarItem>();

        private void Start()
        {
            zoneBarItemFactory.SetParent(zonesContainer);
            ListenEvents();
            BuildZoneBar();
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
            int startZone   = Mathf.Max(ZoneConfig.STARTING_ZONE_NUMBER, currentZone - 1);
            int visibleCount = UIConfig.ZONE_BAR_VISIBLE_COUNT;

            int normalRunEnd = Mathf.Min(
                startZone + visibleCount - 2,
                ZoneConfig.MAX_ZONE_NUMBER - 1
            );

            for (int i = startZone; i <= normalRunEnd; i++)
            {
                AddItem(i, currentZone);
            }

            AddItem(ZoneConfig.MAX_ZONE_NUMBER, currentZone);

            RefreshProgressText(currentZone);
        }

        private void AddItem(int zoneNumber, int currentZone)
        {
            ZoneDTO zoneDTO = zoneController.GetZoneDTO(zoneNumber);
            ZoneBarItem item = zoneBarItemFactory.Create(zoneDTO);
            visibleItems.Add(item);

            if (zoneNumber == currentZone)
            {
                item.SetHighlight(true);
            }
        }

        private void RefreshProgressText(int currentZone)
        {
            if (uiTextZoneCurrentValue == null)
            {
                return;
            }

            uiTextZoneCurrentValue.text = $"{currentZone} / {ZoneConfig.MAX_ZONE_NUMBER}";
        }

        private void HandleZoneChanged(ZoneDTO zoneDTO)
        {
            BuildZoneBar();
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
