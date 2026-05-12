using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public class ZoneBarItemFactory
    {
        private Transform parentTransform;
        private GameObject prefab;
        private AsyncOperationHandle<GameObject> handle;

        private readonly List<ZoneBarItem> activeItems = new List<ZoneBarItem>();

        public void SetParent(Transform parent)
        {
            parentTransform = parent;
        }

        public ZoneBarItem Create(ZoneDTO zoneDTO)
        {
            EnsurePrefabLoaded();
            ZoneBarItem item = Object.Instantiate(prefab, parentTransform, false)
                .GetComponent<ZoneBarItem>();
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
                    Object.Destroy(item.gameObject);
                }
            }

            activeItems.Clear();
        }

        private void EnsurePrefabLoaded()
        {
            if (prefab != null)
            {
                return;
            }

            handle = Addressables.LoadAssetAsync<GameObject>(AddressableConfig.ZONE_BAR_ITEM_PREFAB);
            prefab = handle.WaitForCompletion();
        }

        ~ZoneBarItemFactory()
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}
