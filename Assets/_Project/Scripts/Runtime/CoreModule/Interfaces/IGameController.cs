using System;

namespace GoldenSpin.CoreModule
{
    public interface IGameController
    {
        GameState CurrentState { get; }

        event Action<GameState> OnStateChanged;

        void StartGame();
        
        void RequestSpin();
        
        void OnSpinAnimationCompleted(WheelSliceDTO resultSlice);
        
        void OnSpinResultClosed();

        void RequestCollect();
        
        void RestartGame();

        void ReviveAfterBomb();
    }
}
