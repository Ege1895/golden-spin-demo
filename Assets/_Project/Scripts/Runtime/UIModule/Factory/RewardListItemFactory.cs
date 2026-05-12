using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GoldenSpin.CoreModule;

namespace GoldenSpin.UIModule
{
    public class RewardListItemFactory
    {
        [Inject] 
        private readonly RewardListItem.Factory factory;

        private Transform parentTransform;

        private readonly List<RewardListItem> activeItems = new List<RewardListItem>();

        public void SetParent(Transform parent)
        {
            parentTransform = parent;
        }

        public RewardListItem Create(RewardDTO dto)
        {
            RewardListItem item = factory.Create(dto);
            item.transform.SetParent(parentTransform, false);
            activeItems.Add(item);
            return item;
        }

        public void DestroyAll()
        {
            foreach (RewardListItem item in activeItems)
            {
                if (item != null)
                {
                    Object.Destroy(item.gameObject);
                }
            }

            activeItems.Clear();
        }
    }
}
