using System;
using Zenject;
using GoldenSpin.CoreModule;

namespace GoldenSpin.GameModule
{
    public class GameController : IGameController, IInitializable
    {
        [Inject] private readonly IZoneController   zoneController;
        [Inject] private readonly IWheelController  wheelController;
        [Inject] private readonly IRewardController rewardController;

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
            PrepareAndEnterIdle();
        }

        public void RequestSpin()
        {
            if (CurrentState != GameState.Idle)
            {
                return;
            }

            SetState(GameState.Spinning);

            ZoneDTO zone       = zoneController.CurrentZoneDTO;
            int     multiplier = zoneController.GetCurrentZoneMultiplier();

            wheelController.PrepareWheel(zone.SpinType, multiplier);
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
                zoneController.AdvanceZone();
                PrepareAndEnterIdle();
            }
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
