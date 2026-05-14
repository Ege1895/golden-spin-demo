using UnityEngine;
using UnityEngine.AddressableAssets;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.RewardModule
{
    public class RewardListItemFactory
    {
        public RewardListItem Create(RewardDTO dto, Transform parent)
        {
            GameObject go = Addressables
                .InstantiateAsync(AddressableConfig.REWARD_LIST_ITEM_PREFAB, parent, false)
                .WaitForCompletion();

            RewardListItem item = go.GetComponent<RewardListItem>();
            item.Initialize(dto);
            return item;
        }

        public void Destroy(RewardListItem item)
        {
            if (item == null)
            {
                return;
            }

            Addressables.ReleaseInstance(item.gameObject);
        }
    }
}
