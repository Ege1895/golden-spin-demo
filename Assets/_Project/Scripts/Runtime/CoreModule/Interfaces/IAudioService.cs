namespace GoldenSpin.CoreModule
{
    public interface IAudioService
    {
        bool IsSfxEnabled { get; }
        bool IsMusicEnabled { get; }

        void PlaySfx(string addressableKey);

        void PlayLoop(string addressableKey);

        void StopLoop();

        void SetSfxEnabled(bool enabled);

        void SetMusicEnabled(bool enabled);
    }
}
