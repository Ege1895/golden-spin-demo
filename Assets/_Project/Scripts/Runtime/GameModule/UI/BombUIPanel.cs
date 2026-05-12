using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using GoldenSpin.CoreModule;
using GoldenSpin.UIModule;

namespace GoldenSpin.GameModule
{
    public class BombUIPanel : PopupUIPanel
    {
        [Inject]
        private IGameController gameController;

        [SerializeField]
        private Button uiButtonGiveUp;
        [SerializeField]
        private Button uiButtonRevive;

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

        private void Start()
        {
            ListenEvents();
        }

        private void ListenEvents()
        {
            uiButtonGiveUp.onClick.AddListener(OnGiveUpClicked);
            uiButtonRevive.onClick.AddListener(OnReviveClicked);

            gameController.OnStateChanged += HandleStateChanged;
        }

        protected override void OnShow()
        {
            base.OnShow();
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

        private void OnGiveUpClicked()
        {
            Hide(() => gameController.RestartGame());
        }

        private void OnReviveClicked()
        {
            Hide(() => gameController.ReviveAfterBomb());
        }

        private void UnsubscribeFromEvents()
        {
            uiButtonGiveUp.onClick.RemoveListener(OnGiveUpClicked);
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
            if (uiButtonGiveUp == null)
            {
                uiButtonGiveUp = transform.Find("ui_button_give_up")?.GetComponent<Button>();
            }

            if (uiButtonRevive == null)
            {
                uiButtonRevive = transform.Find("ui_button_revive")?.GetComponent<Button>();
            }
        }
#endif
    }
}
