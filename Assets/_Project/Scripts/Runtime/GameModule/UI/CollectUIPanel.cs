using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.RewardModule;
using GoldenSpin.UIModule;

namespace GoldenSpin.GameModule
{
    public class CollectUIPanel : PopupUIPanel
    {
        [Inject]
        private IGameController gameController;
        [Inject]
        private IRewardController rewardController;
        [Inject]
        private IZoneController zoneController;
        [Inject]
        private RewardListItemFactory rewardListItemFactory;

        [SerializeField]
        private Button uiButtonContinue;

        [SerializeField]
        private Transform rewardScrollContent;

        [SerializeField]
        private TMP_Text uiTextCollectTitle;
        [SerializeField]
        private TMP_Text uiTextTotalRewardsValue;

        private const float REWARD_COUNTER_DURATION = 1.5f;
        private const string TITLE = "REWARDS";

        private readonly List<RewardListItem> summaryItems = new List<RewardListItem>();

#if UNITY_EDITOR
        private const string PATH_BUTTON_CONTINUE = "Popup_Collect/ui_button_continue";
        private const string PATH_REWARD_SCROLL_CONTENT = "Popup_Collect/ui_scroll_rewards/Viewport/Content";
#endif

        protected override void Awake()
        {
            base.Awake();
            ListenEvents();
        }

        private void ListenEvents()
        {
            uiButtonContinue.onClick.AddListener(OnContinueClicked);

            gameController.OnStateChanged += HandleStateChanged;
            rewardController.OnRewardsCollected += HandleRewardsCollected;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Restarting)
            {
                ClearSummary();
                HideImmediate();
            }
        }

        private void HandleRewardsCollected(CollectedRewardDTO collectedDTO)
        {
            Populate(collectedDTO);
            Show();
            PlayCollectAnimation(collectedDTO.Rewards.Count);
        }

        private void Populate(CollectedRewardDTO collectedDTO)
        {
            ClearSummary();

            if (uiTextCollectTitle != null)
            {
                uiTextCollectTitle.text = TITLE;
                uiTextCollectTitle.color = TierColors.For(zoneController.CurrentZoneDTO.SpinType);
            }

            if (uiTextTotalRewardsValue != null)
            {
                uiTextTotalRewardsValue.text = collectedDTO.Rewards.Count.ToString();
            }

            if (rewardScrollContent == null)
            {
                return;
            }

            foreach (RewardDTO reward in collectedDTO.Rewards)
            {
                RewardListItem item = rewardListItemFactory.Create(reward, rewardScrollContent);
                summaryItems.Add(item);
            }
        }

        private void ClearSummary()
        {
            foreach (RewardListItem item in summaryItems)
            {
                rewardListItemFactory.Destroy(item);
            }

            summaryItems.Clear();
        }

        private void PlayCollectAnimation(int target)
        {
            if (uiTextTotalRewardsValue == null)
            {
                return;
            }

            float counter = 0f;

            DOTween.To(
                () => counter,
                x =>
                {
                    counter = x;
                    uiTextTotalRewardsValue.text = Mathf.RoundToInt(counter).ToString();
                },
                target,
                REWARD_COUNTER_DURATION
            ).SetEase(Ease.OutExpo);
        }

        private void OnContinueClicked()
        {
            Hide(() => gameController.RestartGame());
        }

        private void UnsubscribeFromEvents()
        {
            uiButtonContinue.onClick.RemoveListener(OnContinueClicked);

            gameController.OnStateChanged -= HandleStateChanged;
            rewardController.OnRewardsCollected -= HandleRewardsCollected;
        }

        private void OnDestroy()
        {
            ClearSummary();
            UnsubscribeFromEvents();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiButtonContinue == null)
            {
                uiButtonContinue = transform.Find(PATH_BUTTON_CONTINUE)?.GetComponent<Button>();
            }

            if (rewardScrollContent == null)
            {
                rewardScrollContent = transform.Find(PATH_REWARD_SCROLL_CONTENT);
            }
        }
#endif
    }
}
