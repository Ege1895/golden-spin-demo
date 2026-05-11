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
        private Button uiButtonRestart;
        [SerializeField]
        private Button uiButtonRevive;
        
        [SerializeField]
        private TMP_Text uiTextReviveCostValue;
        [SerializeField]
        private TMP_Text uiTextBombTitle;
        [SerializeField]
        private TMP_Text uiTextBombSubtitle;
        
        [SerializeField]
        private Image uiImageBombIcon;
        
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
            uiButtonRestart.onClick.AddListener(OnRestartClicked);
            uiButtonRevive.onClick.AddListener(OnReviveClicked);

            gameController.OnStateChanged += HandleStateChanged;
        }

        private void SetReviveCostText()
        {
            uiTextReviveCostValue.text = REVIVE_COST.ToString();
        }

        protected override void OnShow()
        {
            PlayBombEntryAnimation();
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
            uiButtonRestart.onClick.RemoveListener(OnRestartClicked);
            uiButtonRevive.onClick.RemoveListener(OnReviveClicked);

            gameController.OnStateChanged -= HandleStateChanged;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiButtonRestart == null)
            {
                uiButtonRestart = transform.Find("ui_button_restart")?.GetComponent<Button>();
            }

            if (uiButtonRevive == null)
            {
                uiButtonRevive = transform.Find("ui_button_revive")?.GetComponent<Button>();
            }
        }
#endif
    }
}
