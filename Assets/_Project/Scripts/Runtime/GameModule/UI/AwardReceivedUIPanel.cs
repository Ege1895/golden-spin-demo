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
        private Button uiButtonRevive;
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

        private const string TITLE = "TEBRİKLER!";

        private WheelSliceDTO pendingSpinResult;

        private void Start()
        {
            ListenEvents();
        }

        private void ListenEvents()
        {
            uiButtonRevive.onClick.AddListener(OnContinueClicked);
            uiButtonGiveUp.onClick.AddListener(OnGiveUpClicked);

            gameController.OnStateChanged   += HandleStateChanged;
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
                uiTextCollectTitle.text  = TITLE;
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
            uiButtonRevive.onClick.RemoveListener(OnContinueClicked);
            uiButtonGiveUp.onClick.RemoveListener(OnGiveUpClicked);

            gameController.OnStateChanged   -= HandleStateChanged;
            wheelController.OnSpinCompleted -= HandleSpinCompleted;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}
