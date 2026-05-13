using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public class RewardListItemFactory
    {
        private Transform parentTransform;

        private readonly List<RewardListItem> activeItems = new List<RewardListItem>();

        public void SetParent(Transform parent)
        {
            parentTransform = parent;
        }

        public RewardListItem Create(RewardDTO dto)
        {
            GameObject go = Addressables
                .InstantiateAsync(AddressableConfig.REWARD_LIST_ITEM_PREFAB, parentTransform, false)
                .WaitForCompletion();

            RewardListItem item = go.GetComponent<RewardListItem>();
            item.Initialize(dto);
            activeItems.Add(item);
            return item;
        }

        public void DestroyAll()
        {
            foreach (RewardListItem item in activeItems)
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
