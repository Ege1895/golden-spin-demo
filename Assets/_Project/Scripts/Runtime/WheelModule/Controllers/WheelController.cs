using System;
using System.Collections.Generic;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.ZoneModule;
using Random = UnityEngine.Random;

namespace GoldenSpin.WheelModule
{
    public class WheelController : IWheelController
    {
        [Inject] private readonly IRewardController rewardController;
        [Inject] private readonly ZoneConfigSO      zoneConfigSO;

        private readonly List<WheelSliceDTO> currentSlices = new List<WheelSliceDTO>();

        public IReadOnlyList<WheelSliceDTO> CurrentSlices => currentSlices;

        public event Action<int>           OnSpinStarted;
        public event Action<WheelSliceDTO> OnSpinCompleted;

        public void PrepareWheel(SpinType spinType, int zoneMultiplier)
        {
            currentSlices.Clear();

            WheelConfigSO config = GetWheelConfig(spinType);

            foreach (WheelSliceSO sliceSO in config.Slices)
            {
                currentSlices.Add(sliceSO.ToDTO(zoneMultiplier));
            }
        }

        public int DetermineResultSliceIndex()
        {
            int resultIndex = Random.Range(0, currentSlices.Count);
            OnSpinStarted?.Invoke(resultIndex);
            return resultIndex;
        }

        public void ProcessSpinResult(WheelSliceDTO resultSlice)
        {
            OnSpinCompleted?.Invoke(resultSlice);

            if (resultSlice.ContentType == SliceContentType.Reward)
            {
                rewardController.AddReward(resultSlice.Reward);
            }
            else
            {
                rewardController.ClearRewards();
            }
        }

        public float GetTargetAngleForSlice(int sliceIndex)
        {
            return sliceIndex * WheelConfig.SLICE_ANGLE;
        }

        private WheelConfigSO GetWheelConfig(SpinType spinType)
        {
            switch (spinType)
            {
                case SpinType.Golden:
                    return zoneConfigSO.GoldenWheelConfig;
                case SpinType.Silver:
                    return zoneConfigSO.SilverWheelConfig;
                default:
                    return zoneConfigSO.NormalWheelConfig;
            }
        }
    }
}
