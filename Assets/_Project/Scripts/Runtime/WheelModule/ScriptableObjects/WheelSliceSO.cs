using UnityEngine;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.WheelModule
{
    [CreateAssetMenu(fileName = "WheelSliceSO", menuName = "GoldenSpin/WheelSlice")]
    public class WheelSliceSO : ScriptableObject
    {
        public SliceContentType ContentType;

        public RewardSO Reward;

        [Tooltip("Relative probability weight. Higher = more likely. Default 1.")]
        [Min(WheelConfig.MIN_SLICE_WEIGHT)]
        public int Weight = WheelConfig.DEFAULT_SLICE_WEIGHT;

        public WheelSliceDTO ToDTO(int zoneMultiplier = RewardConfig.DEFAULT_ZONE_MULTIPLIER)
        {
            return new WheelSliceDTO
            {
                ContentType = ContentType,
                Weight      = Weight,
                Reward      = ContentType == SliceContentType.Reward && Reward != null
                    ? Reward.ToDTO(zoneMultiplier)
                    : null
            };
        }
    }
}
