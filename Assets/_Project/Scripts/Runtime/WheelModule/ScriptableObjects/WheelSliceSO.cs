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

        public WheelSliceDTO ToDTO(int zoneMultiplier = RewardConfig.DEFAULT_ZONE_MULTIPLIER)
        {
            return new WheelSliceDTO
            {
                ContentType = ContentType,
                Reward      = ContentType == SliceContentType.Reward && Reward != null
                    ? Reward.ToDTO(zoneMultiplier)
                    : null
            };
        }
    }
}
