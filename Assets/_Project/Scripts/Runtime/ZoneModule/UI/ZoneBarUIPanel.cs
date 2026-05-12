using System.Collections.Generic;
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
            int startZone = Mathf.Max(ZoneConfig.STARTING_ZONE_NUMBER, currentZone - 1);
            int endZone = startZone + UIConfig.ZONE_BAR_VISIBLE_COUNT - 1;

            for (int i = startZone; i <= endZone; i++)
            {
                ZoneDTO zoneDTO = zoneController.GetZoneDTO(i);
                ZoneBarItem item = zoneBarItemFactory.Create(zoneDTO);
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
