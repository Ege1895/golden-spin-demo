namespace GoldenSpin.ConfigurationModule
{
    public static class AudioConfig
    {
        public const string SFX_SPIN_START = "sfx_spin_start";
        public const string SFX_SLICE_TICK = "sfx_slice_tick";
        public const string SFX_SPIN_WIN = "sfx_spin_win";
        public const string SFX_BIG_WIN = "sfx_big_win";
        public const string SFX_BOMB_EXPLOSION = "sfx_bomb_explosion";
        public const string SFX_BUTTON_CLICK = "sfx_button_click";
        public const string SFX_REWARD_COLLECTED = "sfx_reward_collected";
        public const string LOOP_BACKGROUND = "loop_background";
        public const string PREFS_KEY_SFX_ENABLED = "audio_sfx_enabled";
        public const string PREFS_KEY_MUSIC_ENABLED = "audio_music_enabled";
        public const string AUDIO_SERVICE_GO_NAME = "[AudioService]";
        
        public const float DEFAULT_SFX_VOLUME = 1f;
        public const float DEFAULT_MUSIC_VOLUME = 0.6f;
        public const float MUSIC_DUCK_VOLUME = 0.15f;
        public const float LOOP_FADE_DURATION = 0.25f;
        public const float MUSIC_DUCK_FADE_DURATION = 0.3f;
        
        public const int SFX_SOURCE_POOL_SIZE = 4;
        public const int PREFS_VALUE_TRUE = 1;
        public const int PREFS_VALUE_FALSE = 0;

    }
}
