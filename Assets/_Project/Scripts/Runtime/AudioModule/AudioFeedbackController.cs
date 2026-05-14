using System;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.AudioModule
{
    public class AudioFeedbackController : IInitializable, IDisposable
    {
        [Inject]
        private readonly IAudioService audioService;
        
        [Inject]
        private readonly IGameController gameController;
        
        [Inject]
        private readonly IWheelController wheelController;
        
        [Inject]
        private readonly IRewardController rewardController;
        

        public void Initialize()
        {
            ListenEvents();
            audioService.PlayLoop(AudioConfig.LOOP_BACKGROUND);
        }

        private void ListenEvents()
        {
            gameController.OnStateChanged += HandleStateChanged;
            wheelController.OnSpinStarted += HandleSpinStarted;
            wheelController.OnSpinCompleted += HandleSpinCompleted;
            rewardController.OnRewardsCollected += HandleRewardsCollected;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                audioService.DuckMusic();
                audioService.PlaySfx(AudioConfig.SFX_BOMB_EXPLOSION);
                return;
            }

            if (state == GameState.Idle)
            {
                audioService.RestoreMusic();
            }
        }

        private void HandleSpinStarted(int sliceIndex)
        {
            audioService.PlaySfx(AudioConfig.SFX_SPIN_START);
            audioService.DuckMusic();
        }

        private void HandleSpinCompleted(WheelSliceDTO slice)
        {
            audioService.RestoreMusic();

            if (slice == null || slice.ContentType == SliceContentType.Bomb)
            {
                return;
            }

            audioService.PlaySfx(AudioConfig.SFX_SPIN_WIN);
        }

        private void HandleRewardsCollected(CollectedRewardDTO snapshot)
        {
            audioService.PlaySfx(AudioConfig.SFX_REWARD_COLLECTED);
        }

        private void UnsubscribeFromEvents()
        {
            gameController.OnStateChanged -= HandleStateChanged;
            wheelController.OnSpinStarted -= HandleSpinStarted;
            wheelController.OnSpinCompleted -= HandleSpinCompleted;
            rewardController.OnRewardsCollected -= HandleRewardsCollected;
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }
    }
}
