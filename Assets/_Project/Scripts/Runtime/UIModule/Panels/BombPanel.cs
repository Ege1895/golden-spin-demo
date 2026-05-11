using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;

namespace GoldenSpin.UIModule
{
    public class BombPanel : BasePanel
    {
        [Inject]
        private IGameController gameController;

        [SerializeField]
        private Button ui_button_restart;
        [SerializeField]
        private Button ui_button_revive;
        
        [SerializeField]
        private TMP_Text ui_text_revive_cost_value;
        [SerializeField]
        private TMP_Text ui_text_bomb_title;
        [SerializeField]
        private TMP_Text ui_text_bomb_subtitle;
        
        [SerializeField]
        private Image ui_image_bomb_icon;
        
        private const int    REVIVE_COST              = 25;
        private const float  BOMB_ICON_PUNCH_SCALE    = 0.3f;
        private const float  BOMB_ICON_PUNCH_DURATION = 0.4f;
        private const int    BOMB_ICON_PUNCH_VIBRATO  = 5;
        private const float  BOMB_ICON_PUNCH_ELASTIC  = 0.5f;
        private const string LOG_REVIVE_REQUESTED     = "[BombPanel] Revive requested";

        private void Start()
        {
            ListenEvents();
            SetReviveCostText();
        }

        private void ListenEvents()
        {
            ui_button_restart.onClick.AddListener(OnRestartClicked);
            ui_button_revive.onClick.AddListener(OnReviveClicked);

            gameController.OnStateChanged += HandleStateChanged;
        }

        private void SetReviveCostText()
        {
            ui_text_revive_cost_value.text = REVIVE_COST.ToString();
        }

        protected override void OnShow()
        {
            PlayBombEntryAnimation();
        }

        private void PlayBombEntryAnimation()
        {
            ui_image_bomb_icon.transform.localScale = Vector3.one;
            ui_image_bomb_icon.transform.DOPunchScale(
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

        private void OnRestartClicked()
        {
            gameController.RestartGame();
        }

        private void OnReviveClicked()
        {
            Debug.Log(LOG_REVIVE_REQUESTED);
        }

        private void UnsubscribeFromEvents()
        {
            ui_button_restart.onClick.RemoveListener(OnRestartClicked);
            ui_button_revive.onClick.RemoveListener(OnReviveClicked);

            gameController.OnStateChanged -= HandleStateChanged;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_button_restart == null)
            {
                ui_button_restart = transform.Find("ui_button_restart")?.GetComponent<Button>();
            }

            if (ui_button_revive == null)
            {
                ui_button_revive = transform.Find("ui_button_revive")?.GetComponent<Button>();
            }
        }
#endif
    }
}
