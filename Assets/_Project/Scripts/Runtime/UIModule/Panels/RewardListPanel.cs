using UnityEngine;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.UIModule
{
    public class RewardListPanel : BasePanel
    {
        [Inject] 
        private IRewardController     rewardController;
        
        [Inject] 
        private RewardListItemFactory rewardListItemFactory;

        [SerializeField] private Transform rewardsContainer;

        private void Start()
        {
            rewardListItemFactory.SetParent(rewardsContainer);
            ListenEvents();
            ShowImmediate();
        }

        private void ListenEvents()
        {
            rewardController.OnRewardAdded    += HandleRewardAdded;
            rewardController.OnRewardsCleared += HandleRewardsCleared;
        }

        private void HandleRewardAdded(RewardDTO reward)
        {
            DOVirtual.DelayedCall(UIConfig.REWARD_ITEM_APPEAR_DELAY, () =>
            {
                rewardListItemFactory.Create(reward);
            });
        }

        private void HandleRewardsCleared(CollectedRewardDTO snapshot)
        {
            rewardsContainer
                .DOShakePosition(0.4f, new Vector3(8f, 8f, 0f), 8)
                .OnComplete(() => rewardListItemFactory.DestroyAll());
        }

        private void UnsubscribeFromEvents()
        {
            rewardController.OnRewardAdded    -= HandleRewardAdded;
            rewardController.OnRewardsCleared -= HandleRewardsCleared;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}
