using UnityEngine;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.WheelModule
{
    [CreateAssetMenu(fileName = "RewardSO", menuName = "GoldenSpin/Reward")]
    public class RewardSO : ScriptableObject
    {
        public RewardType Type;

        public string Label;
        public string SpriteKey;
        public Sprite Icon;

        public int BaseAmount;

        public RewardDTO ToDTO(int zoneMultiplier = RewardConfig.DEFAULT_ZONE_MULTIPLIER)
        {
            return new RewardDTO
            {
                Type      = Type,
                Label     = Label,
                Amount    = BaseAmount * zoneMultiplier,
                SpriteKey = SpriteKey,
                Icon      = Icon
            };
        }
    }
}
