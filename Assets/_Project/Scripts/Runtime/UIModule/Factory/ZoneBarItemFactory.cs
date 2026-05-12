using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GoldenSpin.CoreModule;

namespace GoldenSpin.UIModule
{
    public class ZoneBarItemFactory
    {
        [Inject] 
        private readonly ZoneBarItem.Factory factory;

        private Transform parentTransform;

        private readonly List<ZoneBarItem> pool = new List<ZoneBarItem>();

        public void SetParent(Transform parent)
        {
            parentTransform = parent;
        }

        public ZoneBarItem Create(ZoneDTO zoneDTO)
        {
            ZoneBarItem item = factory.Create(zoneDTO);
            item.transform.SetParent(parentTransform, false);
            pool.Add(item);
            return item;
        }

        public void DestroyAll()
        {
            foreach (ZoneBarItem item in pool)
            {
                if (item != null)
                {
                    Object.Destroy(item.gameObject);
                }
            }

            pool.Clear();
        }
    }
}
