namespace GoldenSpin.ConfigurationModule
{
    public static class HapticConfig
    {
        public const long DURATION_LIGHT_MS = 10L;
        public const long DURATION_MEDIUM_MS = 25L;
        public const long DURATION_HEAVY_MS = 80L;
        public const long DURATION_TICK_MS = 5L;

        public const int AMPLITUDE_LIGHT = 60;
        public const int AMPLITUDE_MEDIUM = 130;
        public const int AMPLITUDE_HEAVY = 255;
        public const int AMPLITUDE_TICK = 90;

        public const string PREFS_KEY_HAPTIC_ENABLED = "haptic_enabled";

        public const int PREFS_VALUE_TRUE = 1;
        public const int PREFS_VALUE_FALSE = 0;

        public const string ANDROID_API_VERSION_CLASS = "android.os.Build$VERSION";
        public const string ANDROID_SDK_INT_FIELD = "SDK_INT";
        public const int ANDROID_SDK_OREO = 26;

        public const string UNITY_PLAYER_CLASS = "com.unity3d.player.UnityPlayer";
        public const string UNITY_ACTIVITY_FIELD = "currentActivity";
        public const string CONTEXT_GET_SYSTEM_SERVICE = "getSystemService";
        public const string SERVICE_VIBRATOR = "vibrator";
        public const string METHOD_HAS_VIBRATOR = "hasVibrator";
        public const string METHOD_VIBRATE = "vibrate";

        public const string VIBRATION_EFFECT_CLASS = "android.os.VibrationEffect";
        public const string METHOD_CREATE_ONE_SHOT = "createOneShot";
    }
}
