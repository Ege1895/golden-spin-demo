using UnityEngine;
using Zenject;
using GoldenSpin.CoreModule;

namespace GoldenSpin.UIModule
{
    public class WheelSliceItemFactory
    {
        [Inject] 
        private readonly WheelSliceItem.Factory factory;

        private Transform parentTransform;

        public void SetParent(Transform parent)
        {
            parentTransform = parent;
        }

        public WheelSliceItem Create(WheelSliceDTO dto)
        {
            WheelSliceItem item = factory.Create(dto);
            item.transform.SetParent(parentTransform, false);
            return item;
        }

        public void DestroyItem(WheelSliceItem item)
        {
            Object.Destroy(item.gameObject);
        }
    }
}
