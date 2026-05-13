using TMPro;
using UnityEngine;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.UIModule;

namespace GoldenSpin.RewardModule
{
    public class RewardListUIPanel : BasePanel
    {
        [Inject]
        private IRewardController rewardController;

        [Inject]
        private RewardListItemFactory rewardListItemFactory;

        [SerializeField]
        private Transform rewardsContainer;

        [SerializeField]
        private TMP_Text uiTextRewardCountValue;

        private void Start()
        {
            rewardListItemFactory.SetParent(rewardsContainer);
            ListenEvents();
            RefreshCountText();
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
                RefreshCountText();
            });
        }

        private void HandleRewardsCleared(CollectedRewardDTO snapshot)
        {
            rewardsContainer
                .DOShakePosition(0.4f, new Vector3(8f, 8f, 0f), 8)
                .OnComplete(() =>
                {
                    rewardListItemFactory.DestroyAll();
                    RefreshCountText();
                });
        }

        private void RefreshCountText()
        {
            if (uiTextRewardCountValue == null)
            {
                return;
            }

            int count = rewardController.CollectedRewards.Count;
            uiTextRewardCountValue.text = count == 1 ? "1 Item" : $"{count} Items";
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

#if UNITY_EDITOR
        private const string PATH_REWARDS_CONTAINER  = "ui_scroll_rewards/Viewport/Content";
        private const string PATH_TEXT_REWARD_COUNT  = "ui_header/ui_text_reward_count_value";

        private void OnValidate()
        {
            if (rewardsContainer == null)
            {
                rewardsContainer = transform.Find(PATH_REWARDS_CONTAINER);
            }

            if (uiTextRewardCountValue == null)
            {
                uiTextRewardCountValue = transform.Find(PATH_TEXT_REWARD_COUNT)?.GetComponent<TMP_Text>();
            }
        }
#endif
    }
}
