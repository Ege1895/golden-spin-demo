using System.Collections.Generic;
using GoldenSpin.CoreModule;

namespace GoldenSpin.Tests.TestDoubles
{
    public class FakeSaveService : ISaveService
    {
        private readonly Dictionary<string, int> intStore = new Dictionary<string, int>();

        public bool HasKey(string key)
        {
            return intStore.ContainsKey(key);
        }

        public int LoadInt(string key, int defaultValue)
        {
            if (intStore.TryGetValue(key, out int value))
            {
                return value;
            }

            return defaultValue;
        }

        public void SaveInt(string key, int value)
        {
            intStore[key] = value;
        }

        public void DeleteKey(string key)
        {
            intStore.Remove(key);
        }
    }
}
