using System;
using Zenject;
using GoldenSpin.CoreModule;

namespace GoldenSpin.HapticModule
{
    public class HapticFeedbackController : IInitializable, IDisposable
    {
        [Inject]
        private readonly IHapticService hapticService;
        [Inject]
        private readonly IGameController gameController;
        [Inject]
        private readonly IWheelController wheelController;

        public void Initialize()
        {
            ListenEvents();
        }

        private void ListenEvents()
        {
            gameController.OnStateChanged += HandleStateChanged;
            wheelController.OnSpinStarted += HandleSpinStarted;
            wheelController.OnSpinCompleted += HandleSpinCompleted;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                hapticService.Trigger(HapticPattern.Heavy);
            }
        }

        private void HandleSpinStarted(int sliceIndex)
        {
            hapticService.Trigger(HapticPattern.Medium);
        }

        private void HandleSpinCompleted(WheelSliceDTO slice)
        {
            if (slice == null || slice.ContentType == SliceContentType.Bomb)
            {
                return;
            }

            hapticService.Trigger(HapticPattern.Light);
        }

        private void UnsubscribeFromEvents()
        {
            gameController.OnStateChanged -= HandleStateChanged;
            wheelController.OnSpinStarted -= HandleSpinStarted;
            wheelController.OnSpinCompleted -= HandleSpinCompleted;
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }
    }
}
