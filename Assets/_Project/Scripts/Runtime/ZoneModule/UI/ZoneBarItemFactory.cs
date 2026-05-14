using UnityEngine;
using UnityEngine.AddressableAssets;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.ZoneModule
{
    public class ZoneBarItemFactory
    {
        public ZoneBarItem Create(ZoneDTO zoneDTO, Transform parent)
        {
            GameObject go = Addressables
                .InstantiateAsync(AddressableConfig.ZONE_BAR_ITEM_PREFAB, parent, false)
                .WaitForCompletion();

            ZoneBarItem item = go.GetComponent<ZoneBarItem>();
            item.Initialize(zoneDTO);
            return item;
        }

        public void Destroy(ZoneBarItem item)
        {
            if (item == null)
            {
                return;
            }

            Addressables.ReleaseInstance(item.gameObject);
        }
    }
}
