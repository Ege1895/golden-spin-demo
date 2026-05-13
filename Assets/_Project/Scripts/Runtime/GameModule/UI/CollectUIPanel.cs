using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
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

        [SerializeField]
        private Button uiButtonRevive;

        [SerializeField]
        private Transform rewardSummaryContainer;

        [SerializeField]
        private TMP_Text uiTextCollectTitle;
        [SerializeField]
        private TMP_Text uiTextTotalRewardsValue;

        private const float REWARD_COUNTER_DURATION = 1.5f;
        private const string SUMMARY_ITEM_NAME = "ui_text_summary_item";
        private const string TITLE = "ÖDÜLLER";

        private void Start()
        {
            ListenEvents();
        }

        private void ListenEvents()
        {
            uiButtonRevive.onClick.AddListener(OnContinueClicked);

            gameController.OnStateChanged       += HandleStateChanged;
            rewardController.OnRewardsCollected += HandleRewardsCollected;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Restarting)
            {
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
                uiTextCollectTitle.text  = TITLE;
                uiTextCollectTitle.color = TierColors.For(zoneController.CurrentZoneDTO.SpinType);
            }

            if (uiTextTotalRewardsValue != null)
            {
                uiTextTotalRewardsValue.text = collectedDTO.Rewards.Count.ToString();
            }

            if (rewardSummaryContainer == null)
            {
                return;
            }

            foreach (RewardDTO reward in collectedDTO.Rewards)
            {
                GameObject summaryText = new GameObject(SUMMARY_ITEM_NAME);
                summaryText.transform.SetParent(rewardSummaryContainer, false);

                TextMeshProUGUI tmp = summaryText.AddComponent<TextMeshProUGUI>();
                tmp.text = $"{reward.Label}: x{reward.Amount}";
            }
        }

        private void ClearSummary()
        {
            if (rewardSummaryContainer == null)
            {
                return;
            }

            foreach (Transform child in rewardSummaryContainer)
            {
                Object.Destroy(child.gameObject);
            }
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
            uiButtonRevive.onClick.RemoveListener(OnContinueClicked);

            gameController.OnStateChanged       -= HandleStateChanged;
            rewardController.OnRewardsCollected -= HandleRewardsCollected;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}
