namespace GoldenSpin.ConfigurationModule
{
    public static class WheelConfig
    {
        public const int SPIN_SLICE_COUNT = 8;
        public const float SPIN_MIN_DURATION = 3f;
        public const float SPIN_MAX_DURATION = 5f;
        public const float SPIN_MIN_ROTATIONS = 5f;
        public const float SPIN_MAX_ROTATIONS = 10f;
        public const float SPIN_EASE_OVERSHOOT = 0f;
        public const float SLICE_ANGLE = 360f / SPIN_SLICE_COUNT;
        public const float DEGREES_PER_ROTATION = 360f;

        public const int MIN_SLICE_WEIGHT = 0;
        public const int DEFAULT_SLICE_WEIGHT = 1;

        public const float RATCHET_PUNCH_DEGREES = 6f;
        public const float RATCHET_PUNCH_DURATION = 0.06f;
        public const int RATCHET_PUNCH_VIBRATO = 1;
        public const float RATCHET_PUNCH_ELASTICITY = 0.5f;
    }
}
