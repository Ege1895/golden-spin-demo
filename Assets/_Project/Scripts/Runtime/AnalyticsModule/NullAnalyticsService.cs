using System.Collections.Generic;
using GoldenSpin.CoreModule;

namespace GoldenSpin.AnalyticsModule
{
    public class NullAnalyticsService : IAnalyticsService
    {
        public void Track(string eventName)
        {
        }

        public void Track(string eventName, IReadOnlyDictionary<string, object> parameters)
        {
        }
    }
}
