using UnityEngine;
using UnityEngine.AddressableAssets;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.WheelModule
{
    public class WheelSliceItemFactory
    {
        public WheelSliceItem Create(WheelSliceDTO dto, Transform parent)
        {
            GameObject go = Addressables
                .InstantiateAsync(AddressableConfig.WHEEL_SLICE_ITEM_PREFAB, parent, false)
                .WaitForCompletion();

            WheelSliceItem item = go.GetComponent<WheelSliceItem>();
            item.Initialize(dto);
            return item;
        }

        public void Destroy(WheelSliceItem item)
        {
            if (item == null)
            {
                return;
            }

            Addressables.ReleaseInstance(item.gameObject);
        }
    }
}
