namespace GoldenSpin.CoreModule
{
    public enum HapticPattern
    {
        Light,
        Medium,
        Heavy,
        Tick
    }

    public interface IHapticService
    {
        bool IsEnabled { get; }

        void Trigger(HapticPattern pattern);

        void SetEnabled(bool enabled);
    }
}
