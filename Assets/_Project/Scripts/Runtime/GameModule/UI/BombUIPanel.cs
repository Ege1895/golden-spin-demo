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

        private const float BOMB_ICON_PUNCH_SCALE = 0.3f;
        private const float BOMB_ICON_PUNCH_DURATION = 0.4f;
        private const int BOMB_ICON_PUNCH_VIBRATO = 5;
        private const float BOMB_ICON_PUNCH_ELASTIC = 0.5f;

#if UNITY_EDITOR
        private const string PATH_BUTTON_GIVE_UP = "Popup_Bomb/ui_action_row/ui_button_give_up";
        private const string PATH_BUTTON_REVIVE = "Popup_Bomb/ui_action_row/ui_button_revive";
        private const string PATH_BUTTON_REVIVE_PAID = "Popup_Bomb/ui_action_row/ui_button_revive_paid";
        private const string PATH_TEXT_REVIVE_PAID_COST = "Popup_Bomb/ui_action_row/ui_button_revive_paid/ui_text_revive_paid_cost_value";
#endif

        private void Start()
        {
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
                Show();
            }
            else if (state == GameState.Restarting)
            {
                HideImmediate();
            }
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
            Hide(() => gameController.RestartGame());
        }

        private void OnReviveClicked()
        {
            Hide(() => gameController.ReviveAfterBomb());
        }

        private void OnRevivePaidClicked()
        {
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
            UnsubscribeFromEvents();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
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
        }
#endif
    }
}
