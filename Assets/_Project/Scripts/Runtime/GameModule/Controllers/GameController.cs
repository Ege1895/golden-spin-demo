using System;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.GameModule
{
    public class GameController : IGameController, IInitializable
    {
        [Inject]
        private readonly IZoneController    zoneController;
        [Inject]
        private readonly IWheelController   wheelController;
        [Inject]
        private readonly IRewardController  rewardController;
        [Inject]
        private readonly ICurrencyController currencyController;

        public GameState CurrentState { get; private set; }

        public event Action<GameState> OnStateChanged;

        public void Initialize()
        {
            StartGame();
        }

        public void StartGame()
        {
            zoneController.ResetZone();
            rewardController.ClearRewards();
            PrepareCurrentWheel();
            PrepareAndEnterIdle();
        }

        public void RequestSpin()
        {
            if (CurrentState != GameState.Idle)
            {
                return;
            }

            SetState(GameState.Spinning);
            wheelController.DetermineResultSliceIndex();
        }

        public void OnSpinAnimationCompleted(WheelSliceDTO resultSlice)
        {
            if (CurrentState != GameState.Spinning)
            {
                return;
            }

            SetState(GameState.Result);
            wheelController.ProcessSpinResult(resultSlice);

            if (resultSlice.ContentType == SliceContentType.Bomb)
            {
                SetState(GameState.GameOver);
            }
            else
            {
                SetState(GameState.SpinResult);
            }
        }

        public void OnSpinResultClosed()
        {
            zoneController.AdvanceZone();
            PrepareCurrentWheel();
            PrepareAndEnterIdle();
        }

        public void RequestCollect()
        {
            if (CurrentState != GameState.Idle)
            {
                return;
            }

            if (!zoneController.IsCurrentZoneSafeToLeave())
            {
                return;
            }

            SetState(GameState.Collecting);
            rewardController.CollectRewards();
        }

        public void RestartGame()
        {
            SetState(GameState.Restarting);
            StartGame();
        }

        public void ReviveAfterBomb()
        {
            PrepareCurrentWheel();
            PrepareAndEnterIdle();
        }

        public void ReviveWithCurrency()
        {
            if (CurrentState != GameState.GameOver)
            {
                return;
            }

            if (!currencyController.TrySpend(CurrencyConfig.REVIVE_COST))
            {
                return;
            }

            rewardController.RestoreLastSnapshot();
            PrepareCurrentWheel();
            PrepareAndEnterIdle();
        }

        private void PrepareCurrentWheel()
        {
            ZoneDTO zone = zoneController.CurrentZoneDTO;
            wheelController.PrepareWheel(zone.SpinType, zoneController.GetCurrentZoneMultiplier());
        }

        private void PrepareAndEnterIdle()
        {
            SetState(GameState.Idle);
        }

        private void SetState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
