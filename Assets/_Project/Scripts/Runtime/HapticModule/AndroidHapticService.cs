using UnityEngine;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.HapticModule
{
    public class AndroidHapticService : IHapticService, IInitializable
    {
        [Inject]
        private readonly ISaveService saveService;

#if UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject vibrator;
        private AndroidJavaClass vibrationEffectClass;
        private bool useApi26;
#endif

        public bool IsEnabled { get; private set; } = true;

        public void Initialize()
        {
            IsEnabled = saveService.LoadInt(HapticConfig.PREFS_KEY_HAPTIC_ENABLED, HapticConfig.PREFS_VALUE_TRUE) == HapticConfig.PREFS_VALUE_TRUE;
            InitializeVibrator();
        }

        public void Trigger(HapticPattern pattern)
        {
            if (!IsEnabled)
            {
                return;
            }

            long durationMs = GetDuration(pattern);
            int amplitude = GetAmplitude(pattern);

            Vibrate(durationMs, amplitude);
        }

        public void SetEnabled(bool enabled)
        {
            IsEnabled = enabled;
            saveService.SaveInt(HapticConfig.PREFS_KEY_HAPTIC_ENABLED, enabled ? HapticConfig.PREFS_VALUE_TRUE : HapticConfig.PREFS_VALUE_FALSE);
        }

        private long GetDuration(HapticPattern pattern)
        {
            switch (pattern)
            {
                case HapticPattern.Heavy:
                    return HapticConfig.DURATION_HEAVY_MS;
                case HapticPattern.Medium:
                    return HapticConfig.DURATION_MEDIUM_MS;
                case HapticPattern.Light:
                    return HapticConfig.DURATION_LIGHT_MS;
                default:
                    return HapticConfig.DURATION_TICK_MS;
            }
        }

        private int GetAmplitude(HapticPattern pattern)
        {
            switch (pattern)
            {
                case HapticPattern.Heavy:
                    return HapticConfig.AMPLITUDE_HEAVY;
                case HapticPattern.Medium:
                    return HapticConfig.AMPLITUDE_MEDIUM;
                case HapticPattern.Light:
                    return HapticConfig.AMPLITUDE_LIGHT;
                default:
                    return HapticConfig.AMPLITUDE_TICK;
            }
        }

        private void InitializeVibrator()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (AndroidJavaClass versionClass = new AndroidJavaClass(HapticConfig.ANDROID_API_VERSION_CLASS))
            {
                int sdkInt = versionClass.GetStatic<int>(HapticConfig.ANDROID_SDK_INT_FIELD);
                useApi26 = sdkInt >= HapticConfig.ANDROID_SDK_OREO;
            }

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass(HapticConfig.UNITY_PLAYER_CLASS))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>(HapticConfig.UNITY_ACTIVITY_FIELD);
                vibrator = activity.Call<AndroidJavaObject>(HapticConfig.CONTEXT_GET_SYSTEM_SERVICE, HapticConfig.SERVICE_VIBRATOR);
            }

            if (useApi26)
            {
                vibrationEffectClass = new AndroidJavaClass(HapticConfig.VIBRATION_EFFECT_CLASS);
            }
#endif
        }

        private void Vibrate(long durationMs, int amplitude)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (vibrator == null || !vibrator.Call<bool>(HapticConfig.METHOD_HAS_VIBRATOR))
            {
                return;
            }

            if (useApi26 && vibrationEffectClass != null)
            {
                AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(HapticConfig.METHOD_CREATE_ONE_SHOT, durationMs, amplitude);
                vibrator.Call(HapticConfig.METHOD_VIBRATE, effect);
                return;
            }

            vibrator.Call(HapticConfig.METHOD_VIBRATE, durationMs);
#endif
        }
    }
}
