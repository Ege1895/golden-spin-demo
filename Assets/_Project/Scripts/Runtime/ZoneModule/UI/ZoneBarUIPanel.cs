using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.UIModule;

namespace GoldenSpin.ZoneModule
{
    public class ZoneBarUIPanel : MonoBehaviour
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

        private const int ZONE_BAR_LAST_SLOT_OFFSET = 2;
        private const int ZONE_BAR_FIRST_SLOT_OFFSET = 1;
        
        private const string ZONE_PROGRESS_FORMAT = "{0} / {1}";

#if UNITY_EDITOR
        private const string PATH_ZONES_CONTAINER = "ui_scroll_zones/Viewport/Content";
        private const string PATH_TEXT_ZONE_CURRENT = "ui_header/ui_text_zone_current_value";
#endif

        private void Awake()
        {
            ListenEvents();
        }

        private void Start()
        {
            BuildZoneBar();
        }

        private void ListenEvents()
        {
            zoneController.OnZoneChanged += HandleZoneChanged;
        }

        private void BuildZoneBar()
        {
            DestroyAllItems();

            int currentZone = zoneController.CurrentZone;
            int startZone = Mathf.Max(ZoneConfig.STARTING_ZONE_NUMBER, currentZone - ZONE_BAR_FIRST_SLOT_OFFSET);
            int visibleCount = ZoneConfig.ZONE_BAR_VISIBLE_COUNT;

            int normalRunEnd = Mathf.Min(
                startZone + visibleCount - ZONE_BAR_LAST_SLOT_OFFSET,
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
            ZoneBarItem item = zoneBarItemFactory.Create(zoneDTO, zonesContainer);
            visibleItems.Add(item);

            if (zoneNumber == currentZone)
            {
                item.SetHighlight(true);
            }
        }

        private void DestroyAllItems()
        {
            foreach (ZoneBarItem item in visibleItems)
            {
                zoneBarItemFactory.Destroy(item);
            }

            visibleItems.Clear();
        }

        private void RefreshProgressText(int currentZone)
        {
            if (uiTextZoneCurrentValue == null)
            {
                return;
            }

            uiTextZoneCurrentValue.text = string.Format(ZONE_PROGRESS_FORMAT, currentZone, ZoneConfig.MAX_ZONE_NUMBER);
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
            DestroyAllItems();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (zonesContainer == null)
            {
                zonesContainer = transform.Find(PATH_ZONES_CONTAINER);
            }

            if (scrollContent == null)
            {
                scrollContent = transform.Find(PATH_ZONES_CONTAINER)?.GetComponent<RectTransform>();
            }

            if (uiTextZoneCurrentValue == null)
            {
                uiTextZoneCurrentValue = transform.Find(PATH_TEXT_ZONE_CURRENT)?.GetComponent<TMP_Text>();
            }
        }
#endif
    }
}
