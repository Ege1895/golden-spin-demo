using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public class ZoneBarItemFactory
    {
        private Transform parentTransform;

        private readonly List<ZoneBarItem> activeItems = new List<ZoneBarItem>();

        public void SetParent(Transform parent)
        {
            parentTransform = parent;
        }

        public ZoneBarItem Create(ZoneDTO zoneDTO)
        {
            GameObject go = Addressables
                .InstantiateAsync(AddressableConfig.ZONE_BAR_ITEM_PREFAB, parentTransform, false)
                .WaitForCompletion();

            ZoneBarItem item = go.GetComponent<ZoneBarItem>();
            item.Initialize(zoneDTO);
            activeItems.Add(item);
            return item;
        }

        public void DestroyAll()
        {
            foreach (ZoneBarItem item in activeItems)
            {
                if (item != null)
                {
                    Addressables.ReleaseInstance(item.gameObject);
                }
            }

            activeItems.Clear();
        }
    }
}
