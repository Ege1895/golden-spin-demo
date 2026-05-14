using GoldenSpin.CoreModule;

namespace GoldenSpin.HapticModule
{
    public class NullHapticService : IHapticService
    {
        public bool IsEnabled => false;

        public void Trigger(HapticPattern pattern)
        {
        }

        public void SetEnabled(bool enabled)
        {
        }
    }
}
