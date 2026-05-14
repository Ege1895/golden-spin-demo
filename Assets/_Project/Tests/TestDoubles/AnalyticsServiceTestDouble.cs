using System.Collections.Generic;
using GoldenSpin.CoreModule;

namespace GoldenSpin.Tests.TestDoubles
{
    public class AnalyticsServiceTestDouble : IAnalyticsService
    {
        public List<string> TrackedEvents { get; } = new List<string>();
        public List<IReadOnlyDictionary<string, object>> TrackedParameters { get; } = new List<IReadOnlyDictionary<string, object>>();

        public void Track(string eventName)
        {
            TrackedEvents.Add(eventName);
            TrackedParameters.Add(null);
        }

        public void Track(string eventName, IReadOnlyDictionary<string, object> parameters)
        {
            TrackedEvents.Add(eventName);
            TrackedParameters.Add(parameters);
        }
    }
}
