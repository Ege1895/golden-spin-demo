using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.UIModule;

namespace GoldenSpin.GameModule
{
    public class AwardReceivedUIPanel : PopupUIPanel
    {
        [Inject]
        private IGameController gameController;

        [Inject]
        private IWheelController wheelController;

        [Inject]
        private IZoneController zoneController;

        [SerializeField]
        private Button uiButtonContinue;
        [SerializeField]
        private Button uiButtonGiveUp;

        [SerializeField]
        private TMP_Text uiTextCollectTitle;
        [SerializeField]
        private TMP_Text uiTextTotalRewardsValue;

        [SerializeField]
        private Image uiImageRewardIcon;
        [SerializeField]
        private AspectRatioFitter rewardIconFitter;

        private const string TITLE = "CONGRATULATIONS!";

        private WheelSliceDTO pendingSpinResult;

#if UNITY_EDITOR
        private const string PATH_BUTTON_CONTINUE = "Popup_AwardReceived/ui_button_continue";
        private const string PATH_BUTTON_GIVE_UP = "Popup_AwardReceived/ui_button_give_up";
        private const string PATH_TEXT_COLLECT_TITLE = "Popup_AwardReceived/ui_text_collect_title";
        private const string PATH_TEXT_TOTAL_REWARDS = "Popup_AwardReceived/ui_text_total_rewards_value";
        private const string PATH_IMAGE_REWARD_ICON = "Popup_AwardReceived/ui_image_reward_icon";
#endif

        private void Start()
        {
            ListenEvents();
        }

        private void ListenEvents()
        {
            uiButtonContinue.onClick.AddListener(OnContinueClicked);
            uiButtonGiveUp.onClick.AddListener(OnGiveUpClicked);

            gameController.OnStateChanged += HandleStateChanged;
            wheelController.OnSpinCompleted += HandleSpinCompleted;
        }

        private void HandleSpinCompleted(WheelSliceDTO slice)
        {
            pendingSpinResult = slice;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.SpinResult)
            {
                Populate(pendingSpinResult);
                Show();
            }
            else if (state == GameState.Restarting)
            {
                pendingSpinResult = null;
                HideImmediate();
            }
        }

        private void Populate(WheelSliceDTO slice)
        {
            if (uiTextCollectTitle != null)
            {
                uiTextCollectTitle.text = TITLE;
                uiTextCollectTitle.color = TierColors.For(zoneController.CurrentZoneDTO.SpinType);
            }

            bool hasReward = slice != null && slice.Reward != null;

            if (uiImageRewardIcon != null)
            {
                bool showIcon = hasReward && slice.Reward.Icon != null;
                uiImageRewardIcon.gameObject.SetActive(showIcon);

                if (showIcon)
                {
                    SetRewardIcon(slice.Reward.Icon);
                }
            }

            if (uiTextTotalRewardsValue != null)
            {
                uiTextTotalRewardsValue.text = hasReward ? $"x{slice.Reward.Amount}" : string.Empty;
            }
        }

        private void SetRewardIcon(Sprite sprite)
        {
            uiImageRewardIcon.sprite = sprite;

            if (rewardIconFitter != null)
            {
                rewardIconFitter.aspectRatio = sprite.rect.width / sprite.rect.height;
            }
        }

        private void OnContinueClicked()
        {
            Hide(() => gameController.OnSpinResultClosed());
        }

        private void OnGiveUpClicked()
        {
            Hide(() => gameController.RestartGame());
        }

        private void UnsubscribeFromEvents()
        {
            uiButtonContinue.onClick.RemoveListener(OnContinueClicked);
            uiButtonGiveUp.onClick.RemoveListener(OnGiveUpClicked);

            gameController.OnStateChanged -= HandleStateChanged;
            wheelController.OnSpinCompleted -= HandleSpinCompleted;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiButtonContinue == null)
            {
                uiButtonContinue = transform.Find(PATH_BUTTON_CONTINUE)?.GetComponent<Button>();
            }

            if (uiButtonGiveUp == null)
            {
                uiButtonGiveUp = transform.Find(PATH_BUTTON_GIVE_UP)?.GetComponent<Button>();
            }

            if (uiTextCollectTitle == null)
            {
                uiTextCollectTitle = transform.Find(PATH_TEXT_COLLECT_TITLE)?.GetComponent<TMP_Text>();
            }

            if (uiTextTotalRewardsValue == null)
            {
                uiTextTotalRewardsValue = transform.Find(PATH_TEXT_TOTAL_REWARDS)?.GetComponent<TMP_Text>();
            }

            if (uiImageRewardIcon == null)
            {
                uiImageRewardIcon = transform.Find(PATH_IMAGE_REWARD_ICON)?.GetComponent<Image>();
            }

            if (rewardIconFitter == null)
            {
                rewardIconFitter = transform.Find(PATH_IMAGE_REWARD_ICON)?.GetComponent<AspectRatioFitter>();
            }
        }
#endif
    }
}
