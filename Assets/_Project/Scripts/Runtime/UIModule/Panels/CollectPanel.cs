using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;

namespace GoldenSpin.UIModule
{
    public class CollectPanel : BasePanel
    {

        [Inject]
        private IGameController gameController;
        [Inject]
        private IRewardController rewardController;

        [SerializeField]
        private Button uiButtonPlayAgain;
        
        [SerializeField]
        private Transform rewardSummaryContainer;
        
        [SerializeField]
        private TMP_Text uiTextCollectTitle;
        [SerializeField]
        private TMP_Text uiTextTotalRewardsValue;
        
        [SerializeField]
        private GameObject confettiEffect;

        private const float  REWARD_COUNTER_DURATION = 1.5f;
        private const float  CONFETTI_DELAY          = 0.3f;
        private const string SUMMARY_ITEM_NAME       = "ui_text_summary_item";

        private bool isSpinResult;
        
        private void Start()
        {
            ListenEvents();
        }

        private void ListenEvents()
        {
            uiButtonPlayAgain.onClick.AddListener(OnPlayAgainClicked);

            gameController.OnStateChanged       += HandleStateChanged;
            rewardController.OnRewardsCollected += HandleRewardsCollected;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.SpinResult)
            {
                isSpinResult = true;
                Show();
            }
            else if (state == GameState.Restarting)
            {
                isSpinResult = false;
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

        protected override void OnShow()
        {
            if (confettiEffect != null)
            {
                confettiEffect.SetActive(false);
                DOVirtual.DelayedCall(CONFETTI_DELAY, () => confettiEffect.SetActive(true));
            }
        }

        private void PopulateRewardSummary(CollectedRewardDTO collectedDTO)
        {
            ClearRewardSummary();

            int totalCount = collectedDTO.Rewards.Count;
            uiTextTotalRewardsValue.text = totalCount.ToString();

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

        private void OnPlayAgainClicked()
        {
            if (isSpinResult)
            {
                Hide(() => gameController.OnSpinResultClosed());
            }
            else
            {
                Hide(() => gameController.RestartGame());
            }
        }

        private void UnsubscribeFromEvents()
        {
            uiButtonPlayAgain.onClick.RemoveListener(OnPlayAgainClicked);

            gameController.OnStateChanged       -= HandleStateChanged;
            rewardController.OnRewardsCollected -= HandleRewardsCollected;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiButtonPlayAgain == null)
            {
                uiButtonPlayAgain = transform.Find("ui_button_play_again")?.GetComponent<Button>();
            }
        }
#endif
    }
}
