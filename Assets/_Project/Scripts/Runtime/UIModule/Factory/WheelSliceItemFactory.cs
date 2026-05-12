using UnityEngine;
using UnityEngine.AddressableAssets;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public class WheelSliceItemFactory
    {
        private Transform parentTransform;

        public void SetParent(Transform parent)
        {
            parentTransform = parent;
        }

        public WheelSliceItem Create(WheelSliceDTO dto)
        {
            GameObject go = Addressables
                .InstantiateAsync(AddressableConfig.WHEEL_SLICE_ITEM_PREFAB, parentTransform, false)
                .WaitForCompletion();

            WheelSliceItem item = go.GetComponent<WheelSliceItem>();
            item.Initialize(dto);
            return item;
        }

        public void DestroyItem(WheelSliceItem item)
        {
            Addressables.ReleaseInstance(item.gameObject);
        }
    }
}
