using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public class RewardListItemFactory
    {
        private Transform parentTransform;
        private GameObject prefab;
        private AsyncOperationHandle<GameObject> handle;

        private readonly List<RewardListItem> activeItems = new List<RewardListItem>();

        public void SetParent(Transform parent)
        {
            parentTransform = parent;
        }

        public RewardListItem Create(RewardDTO dto)
        {
            EnsurePrefabLoaded();
            RewardListItem item = Object.Instantiate(prefab, parentTransform, false)
                .GetComponent<RewardListItem>();
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

            handle = Addressables.LoadAssetAsync<GameObject>(AddressableConfig.REWARD_LIST_ITEM_PREFAB);
            prefab = handle.WaitForCompletion();
        }

        ~RewardListItemFactory()
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}
