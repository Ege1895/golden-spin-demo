using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public class WheelSliceItemFactory
    {
        private Transform parentTransform;
        private GameObject prefab;
        private AsyncOperationHandle<GameObject> handle;

        public void SetParent(Transform parent)
        {
            parentTransform = parent;
        }

        public WheelSliceItem Create(WheelSliceDTO dto)
        {
            EnsurePrefabLoaded();
            WheelSliceItem item = Object.Instantiate(prefab, parentTransform, false)
                .GetComponent<WheelSliceItem>();
            item.Initialize(dto);
            return item;
        }

        public void DestroyItem(WheelSliceItem item)
        {
            Object.Destroy(item.gameObject);
        }

        private void EnsurePrefabLoaded()
        {
            if (prefab != null)
            {
                return;
            }

            handle = Addressables.LoadAssetAsync<GameObject>(AddressableConfig.WHEEL_SLICE_ITEM_PREFAB);
            prefab = handle.WaitForCompletion();
        }

        ~WheelSliceItemFactory()
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}
