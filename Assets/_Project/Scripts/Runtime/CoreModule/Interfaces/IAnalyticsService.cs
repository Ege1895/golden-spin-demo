using System.Collections.Generic;

namespace GoldenSpin.CoreModule
{
    public interface IAnalyticsService
    {
        void Track(string eventName);

        void Track(string eventName, IReadOnlyDictionary<string, object> parameters);
    }
}
