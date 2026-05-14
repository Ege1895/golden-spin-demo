using UnityEngine;
using GoldenSpin.CoreModule;

namespace GoldenSpin.SaveModule
{
    public class PlayerPrefsSaveService : ISaveService
    {
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public int LoadInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }
}
