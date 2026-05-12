using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;

namespace GoldenSpin.UIModule
{
    public class CollectPanel : PopupUIPanel
    {
        [Inject]
        private IGameController gameController;
        [Inject]
        private IRewardController rewardController;
        [Inject]
        private IWheelController wheelController;

        [SerializeField]
        private Button uiButtonRevive;
        [SerializeField]
        private Button uiButtonGiveUp;

        [SerializeField]
        private Transform rewardSummaryContainer;

        [SerializeField]
        private TMP_Text uiTextCollectTitle;
        [SerializeField]
        private TMP_Text uiTextTotalRewardsValue;
        [SerializeField]
        private Image uiImageRewardIcon;
        [SerializeField]
        private AspectRatioFitter rewardIconFitter;

        private const float  REWARD_COUNTER_DURATION = 1.5f;
        private const float  CONFETTI_DELAY = 0.3f;
        private const string SUMMARY_ITEM_NAME = "ui_text_summary_item";
        private const string TITLE_SPIN_RESULT = "TEBRİKLER!";
        private const string TITLE_COLLECT = "ÖDÜLLER";
        
        private WheelSliceDTO pendingSpinResult;
        
        private bool isSpinResult;

        private void Start()
        {
            ListenEvents();
        }

        private void ListenEvents()
        {
            uiButtonRevive.onClick.AddListener(OnReviveClicked);
            uiButtonGiveUp.onClick.AddListener(OnGiveUpClicked);

            gameController.OnStateChanged       += HandleStateChanged;
            rewardController.OnRewardsCollected += HandleRewardsCollected;
            wheelController.OnSpinCompleted     += HandleSpinCompleted;
        }

        private void HandleSpinCompleted(WheelSliceDTO slice)
        {
            pendingSpinResult = slice;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.SpinResult)
            {
                isSpinResult = true;
                PopulateSpinResult(pendingSpinResult);
                Show();
            }
            else if (state == GameState.Restarting)
            {
                isSpinResult      = false;
                pendingSpinResult = null;
                HideImmediate();
            }
        }

        private void HandleRewardsCollected(CollectedRewardDTO collectedDTO)
        {
            isSpinResult = false;
            PopulateRewardSummary(collectedDTO);
            Show();
            PlayCollectAnimation();
        }

        private void PopulateSpinResult(WheelSliceDTO slice)
        {
            ClearRewardSummary();

            uiTextCollectTitle.text = TITLE_SPIN_RESULT;

            bool hasReward = slice != null && slice.Reward != null;

            if (uiImageRewardIcon != null)
            {
                bool showIcon = hasReward && slice.Reward.Icon != null;
                uiImageRewardIcon.gameObject.SetActive(showIcon);
                if (showIcon)
                    SetRewardIcon(slice.Reward.Icon);
            }

            uiTextTotalRewardsValue.text = hasReward ? $"x{slice.Reward.Amount}" : string.Empty;
        }

        private void SetRewardIcon(Sprite sprite)
        {
            uiImageRewardIcon.sprite = sprite;
            if (rewardIconFitter != null)
                rewardIconFitter.aspectRatio = sprite.rect.width / sprite.rect.height;
        }

        private void PopulateRewardSummary(CollectedRewardDTO collectedDTO)
        {
            ClearRewardSummary();

            uiTextCollectTitle.text = TITLE_COLLECT;

            if (uiImageRewardIcon != null)
                uiImageRewardIcon.gameObject.SetActive(false);

            uiTextTotalRewardsValue.text = collectedDTO.Rewards.Count.ToString();

            foreach (RewardDTO reward in collectedDTO.Rewards)
            {
                GameObject summaryText = new GameObject(SUMMARY_ITEM_NAME);
                summaryText.transform.SetParent(rewardSummaryContainer, false);
                
                TextMeshProUGUI tmp = summaryText.AddComponent<TextMeshProUGUI>();
                tmp.text = $"{reward.Label}: x{reward.Amount}";
            }
        }

        private void ClearRewardSummary()
        {
            foreach (Transform child in rewardSummaryContainer)
            {
                Object.Destroy(child.gameObject);
            }
        }

        private void PlayCollectAnimation()
        {
            int   target  = rewardController.CollectedRewards.Count;
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

        private void OnReviveClicked()
        {
            if (isSpinResult)
                Hide(() => gameController.OnSpinResultClosed());
            else
                Hide(() => gameController.RestartGame());
        }

        private void OnGiveUpClicked()
        {
            Hide(() => gameController.RestartGame());
        }

        private void UnsubscribeFromEvents()
        {
            uiButtonRevive.onClick.RemoveListener(OnReviveClicked);
            uiButtonGiveUp.onClick.RemoveListener(OnGiveUpClicked);

            gameController.OnStateChanged       -= HandleStateChanged;
            rewardController.OnRewardsCollected -= HandleRewardsCollected;
            wheelController.OnSpinCompleted     -= HandleSpinCompleted;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiButtonRevive == null)
                uiButtonRevive = transform.Find("ui_button_revive")?.GetComponent<Button>();
            if (uiButtonGiveUp == null)
                uiButtonGiveUp = transform.Find("ui_button_give_up")?.GetComponent<Button>();
        }
#endif
    }
}
