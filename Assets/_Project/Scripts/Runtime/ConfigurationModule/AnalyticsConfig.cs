namespace GoldenSpin.ConfigurationModule
{
    public static class AnalyticsConfig
    {
        public const string EVENT_GAME_STARTED = "game_started";
        public const string EVENT_SPIN_REQUESTED = "spin_requested";
        public const string EVENT_SPIN_COMPLETED_REWARD = "spin_completed_reward";
        public const string EVENT_SPIN_COMPLETED_BOMB = "spin_completed_bomb";
        public const string EVENT_ZONE_ADVANCED = "zone_advanced";
        public const string EVENT_REWARDS_COLLECTED = "rewards_collected";
        public const string EVENT_BOMB_REVIVE_AD = "bomb_revive_ad";
        public const string EVENT_BOMB_REVIVE_PAID = "bomb_revive_paid";
        public const string EVENT_BOMB_GIVE_UP = "bomb_give_up";
        public const string EVENT_GAME_RESTARTED = "game_restarted";

        public const string PARAM_ZONE_NUMBER = "zone";
        public const string PARAM_SPIN_TYPE = "spin_type";
        public const string PARAM_REWARD_TYPE = "reward_type";
        public const string PARAM_REWARD_AMOUNT = "reward_amount";
        public const string PARAM_TOTAL_REWARDS = "total_rewards";
        public const string PARAM_REVIVE_COST = "revive_cost";
    }
}
