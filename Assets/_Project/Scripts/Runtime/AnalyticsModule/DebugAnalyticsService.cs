using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GoldenSpin.CoreModule;

namespace GoldenSpin.AnalyticsModule
{
    public class DebugAnalyticsService : IAnalyticsService
    {
        private const string LOG_PREFIX = "[Analytics] ";
        private const string PARAM_SEPARATOR = " | ";
        private const string KEY_VALUE_FORMAT = "{0}={1}";

        public void Track(string eventName)
        {
            Debug.Log(LOG_PREFIX + eventName);
        }

        public void Track(string eventName, IReadOnlyDictionary<string, object> parameters)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(LOG_PREFIX);
            builder.Append(eventName);

            if (parameters == null)
            {
                Debug.Log(builder.ToString());
                return;
            }

            foreach (KeyValuePair<string, object> entry in parameters)
            {
                builder.Append(PARAM_SEPARATOR);
                builder.AppendFormat(KEY_VALUE_FORMAT, entry.Key, entry.Value);
            }

            Debug.Log(builder.ToString());
        }
    }
}
