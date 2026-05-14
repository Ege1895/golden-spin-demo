using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.UIModule;

namespace GoldenSpin.RewardModule
{
    public class RewardListUIPanel : MonoBehaviour
    {
        [Inject]
        private IRewardController rewardController;

        [Inject]
        private RewardListItemFactory rewardListItemFactory;

        [SerializeField]
        private Transform rewardsContainer;

        [SerializeField]
        private TMP_Text uiTextRewardCountValue;

        private readonly List<RewardListItem> activeItems = new List<RewardListItem>();
        
        private const int SHAKE_VIBRATO = 8;
        
        private const float SHAKE_DURATION = 0.4f;
        private const float SHAKE_STRENGTH = 8f;
        
        private const string ITEM_COUNT_SINGLE = "1 Item";
        private const string ITEM_COUNT_PLURAL_FORMAT = "{0} Items";

#if UNITY_EDITOR
        private const string PATH_REWARDS_CONTAINER = "ui_scroll_rewards/Viewport/Content";
        private const string PATH_TEXT_REWARD_COUNT = "ui_header/ui_text_reward_count_value";
#endif

        private void Awake()
        {
            ListenEvents();
        }

        private void Start()
        {
            RefreshCountText();
        }

        private void ListenEvents()
        {
            rewardController.OnRewardAdded += HandleRewardAdded;
            rewardController.OnRewardsCleared += HandleRewardsCleared;
        }

        private void HandleRewardAdded(RewardDTO reward)
        {
            DOVirtual.DelayedCall(UIConfig.REWARD_ITEM_APPEAR_DELAY, () =>
            {
                RewardListItem item = rewardListItemFactory.Create(reward, rewardsContainer);
                activeItems.Add(item);
                RefreshCountText();
            });
        }

        private void HandleRewardsCleared(CollectedRewardDTO snapshot)
        {
            rewardsContainer
                .DOShakePosition(SHAKE_DURATION, new Vector3(SHAKE_STRENGTH, SHAKE_STRENGTH, 0f), SHAKE_VIBRATO)
                .OnComplete(() =>
                {
                    DestroyAllItems();
                    RefreshCountText();
                });
        }

        private void DestroyAllItems()
        {
            foreach (RewardListItem item in activeItems)
            {
                rewardListItemFactory.Destroy(item);
            }

            activeItems.Clear();
        }

        private void RefreshCountText()
        {
            if (uiTextRewardCountValue == null)
            {
                return;
            }

            int count = rewardController.CollectedRewards.Count;
            uiTextRewardCountValue.text = count == 1 ? ITEM_COUNT_SINGLE : string.Format(ITEM_COUNT_PLURAL_FORMAT, count);
        }

        private void UnsubscribeFromEvents()
        {
            rewardController.OnRewardAdded -= HandleRewardAdded;
            rewardController.OnRewardsCleared -= HandleRewardsCleared;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
            DestroyAllItems();
        }

#if UNITY_EDITOR
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
