using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.UIModule;

namespace GoldenSpin.GameModule
{
    public class BombUIPanel : PopupUIPanel
    {
        [Inject]
        private IGameController gameController;
        
        [Inject]
        private ICurrencyController currencyController;

        [SerializeField]
        private Button uiButtonGiveUp;
        [SerializeField]
        private Button uiButtonRevive;
        [SerializeField]
        private Button uiButtonRevivePaid;

        [SerializeField]
        private TMP_Text uiTextRevivePaidCostValue;
        [SerializeField]
        private TMP_Text uiTextBombTitle;
        [SerializeField]
        private TMP_Text uiTextBombSubtitle;

        [SerializeField]
        private Image uiImageBombIcon;
        [SerializeField]
        private Image uiImageCountdownFill;

        private const float BOMB_ICON_PUNCH_SCALE = 0.3f;
        private const float BOMB_ICON_PUNCH_DURATION = 0.4f;
        private const float BOMB_ICON_PUNCH_ELASTIC = 0.5f;
        private const float SHOW_DELAY_AFTER_EXPLOSION = 0.45f;
        private const float COUNTDOWN_DURATION_SECONDS = 5f;
        private const float COUNTDOWN_FILL_FULL = 1f;
        private const float COUNTDOWN_FILL_EMPTY = 0f;
        
        private const int BOMB_ICON_PUNCH_VIBRATO = 5;
        
        private const string COUNTDOWN_FORMAT = "{0}";

#if UNITY_EDITOR
        private const string PATH_BUTTON_GIVE_UP = "Popup_Bomb/ui_action_row/ui_button_give_up";
        private const string PATH_BUTTON_REVIVE = "Popup_Bomb/ui_action_row/ui_button_revive";
        private const string PATH_BUTTON_REVIVE_PAID = "Popup_Bomb/ui_action_row/ui_button_revive_paid";
        private const string PATH_TEXT_REVIVE_PAID_COST = "Popup_Bomb/ui_action_row/ui_button_revive_paid/ui_text_revive_paid_cost_value";
        private const string PATH_TEXT_COUNTDOWN = "Popup_Bomb/ui_text_countdown_value";
        private const string PATH_IMAGE_COUNTDOWN_FILL = "Popup_Bomb/ui_image_countdown_fill";
#endif

        private Tween showDelayTween;
        private Tween countdownTween;
        
        private float countdownRemaining;
        
        private bool autoGiveUpTriggered;

        protected override void Awake()
        {
            base.Awake();
            ListenEvents();
        }

        private void ListenEvents()
        {
            uiButtonGiveUp.onClick.AddListener(OnGiveUpClicked);
            uiButtonRevive.onClick.AddListener(OnReviveClicked);
            uiButtonRevivePaid.onClick.AddListener(OnRevivePaidClicked);

            gameController.OnStateChanged += HandleStateChanged;
            currencyController.OnCurrencyChanged += RefreshPaidReviveButton;
        }

        protected override void OnShow()
        {
            base.OnShow();
            PlayBombEntryAnimation();
            RefreshPaidReviveButton(currencyController.CurrentAmount);
            StartCountdown();
        }

        protected override void OnHide()
        {
            base.OnHide();
            StopCountdown();
        }

        private void PlayBombEntryAnimation()
        {
            uiImageBombIcon.transform.localScale = Vector3.one;
            uiImageBombIcon.transform.DOPunchScale(
                Vector3.one * BOMB_ICON_PUNCH_SCALE,
                BOMB_ICON_PUNCH_DURATION,
                BOMB_ICON_PUNCH_VIBRATO,
                BOMB_ICON_PUNCH_ELASTIC
            );
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                ScheduleDelayedShow();
                return;
            }

            if (state == GameState.Restarting)
            {
                CancelDelayedShow();
                StopCountdown();
                HideImmediate();
            }
        }

        private void ScheduleDelayedShow()
        {
            CancelDelayedShow();
            autoGiveUpTriggered = false;
            showDelayTween = DOVirtual.DelayedCall(SHOW_DELAY_AFTER_EXPLOSION, Show);
        }

        private void CancelDelayedShow()
        {
            showDelayTween?.Kill();
            showDelayTween = null;
        }

        private void StartCountdown()
        {
            StopCountdown();
            countdownRemaining = COUNTDOWN_DURATION_SECONDS;
            UpdateCountdownVisuals();

            countdownTween = DOTween.To(
                () => countdownRemaining,
                value =>
                {
                    countdownRemaining = value;
                    UpdateCountdownVisuals();
                },
                0f,
                COUNTDOWN_DURATION_SECONDS
            ).SetEase(Ease.Linear).OnComplete(HandleCountdownExpired);
        }

        private void StopCountdown()
        {
            countdownTween?.Kill();
            countdownTween = null;
        }

        private void UpdateCountdownVisuals()
        {
            if (uiImageCountdownFill != null)
            {
                uiImageCountdownFill.fillAmount = Mathf.Lerp(
                    COUNTDOWN_FILL_EMPTY,
                    COUNTDOWN_FILL_FULL,
                    countdownRemaining / COUNTDOWN_DURATION_SECONDS
                );
            }
        }

        private void HandleCountdownExpired()
        {
            if (autoGiveUpTriggered)
            {
                return;
            }

            autoGiveUpTriggered = true;
            OnGiveUpClicked();
        }

        private void RefreshPaidReviveButton(int amount)
        {
            bool canAfford = currencyController.CanAfford(CurrencyConfig.REVIVE_COST);
            uiButtonRevivePaid.interactable = canAfford;

            if (uiTextRevivePaidCostValue != null)
            {
                uiTextRevivePaidCostValue.text = CurrencyConfig.REVIVE_COST.ToString();
            }
        }

        private void OnGiveUpClicked()
        {
            StopCountdown();
            Hide(() => gameController.RestartGame());
        }

        private void OnReviveClicked()
        {
            StopCountdown();
            Hide(() => gameController.ReviveAfterBomb());
        }

        private void OnRevivePaidClicked()
        {
            StopCountdown();
            Hide(() => gameController.ReviveWithCurrency());
        }

        private void UnsubscribeFromEvents()
        {
            uiButtonGiveUp.onClick.RemoveListener(OnGiveUpClicked);
            uiButtonRevive.onClick.RemoveListener(OnReviveClicked);
            uiButtonRevivePaid.onClick.RemoveListener(OnRevivePaidClicked);

            gameController.OnStateChanged -= HandleStateChanged;
            currencyController.OnCurrencyChanged -= RefreshPaidReviveButton;
        }

        private void OnDestroy()
        {
            CancelDelayedShow();
            StopCountdown();
            UnsubscribeFromEvents();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (uiButtonGiveUp == null)
            {
                uiButtonGiveUp = transform.Find(PATH_BUTTON_GIVE_UP)?.GetComponent<Button>();
            }

            if (uiButtonRevive == null)
            {
                uiButtonRevive = transform.Find(PATH_BUTTON_REVIVE)?.GetComponent<Button>();
            }

            if (uiButtonRevivePaid == null)
            {
                uiButtonRevivePaid = transform.Find(PATH_BUTTON_REVIVE_PAID)?.GetComponent<Button>();
            }

            if (uiTextRevivePaidCostValue == null)
            {
                uiTextRevivePaidCostValue = transform.Find(PATH_TEXT_REVIVE_PAID_COST)?.GetComponent<TMP_Text>();
            }

            if (uiImageCountdownFill == null)
            {
                uiImageCountdownFill = transform.Find(PATH_IMAGE_COUNTDOWN_FILL)?.GetComponent<Image>();
            }
        }
#endif
    }
}
