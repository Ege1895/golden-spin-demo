using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.ZoneModule;
using Random = UnityEngine.Random;

namespace GoldenSpin.WheelModule
{
    public class WheelController : IWheelController
    {
        [Inject]
        private readonly IRewardController rewardController;

        [Inject]
        private readonly ZoneConfigSO zoneConfigSO;

        private readonly List<WheelSliceDTO> currentSlices = new List<WheelSliceDTO>();

        public IReadOnlyList<WheelSliceDTO> CurrentSlices => currentSlices;

        public event Action<SpinType> OnWheelPrepared;

        public event Action<int> OnSpinStarted;

        public event Action<WheelSliceDTO> OnSpinCompleted;

        public void PrepareWheel(SpinType spinType, int zoneMultiplier)
        {
            currentSlices.Clear();

            WheelConfigSO config = GetWheelConfig(spinType);
            int effectiveMultiplier = zoneMultiplier * GetSpinTypeMultiplier(spinType);

            foreach (WheelSliceSO sliceSO in config.Slices)
            {
                currentSlices.Add(sliceSO.ToDTO(effectiveMultiplier));
            }

            OnWheelPrepared?.Invoke(spinType);
        }

        public int DetermineResultSliceIndex()
        {
            int resultIndex = PickWeightedIndex();
            OnSpinStarted?.Invoke(resultIndex);
            return resultIndex;
        }

        public void ProcessSpinResult(WheelSliceDTO resultSlice)
        {
            OnSpinCompleted?.Invoke(resultSlice);

            if (resultSlice.ContentType == SliceContentType.Reward)
            {
                rewardController.AddReward(resultSlice.Reward);
                return;
            }

            rewardController.ClearRewards();
        }

        public float GetTargetAngleForSlice(int sliceIndex)
        {
            return sliceIndex * WheelConfig.SLICE_ANGLE;
        }

        private int PickWeightedIndex()
        {
            int totalWeight = 0;

            for (int i = 0; i < currentSlices.Count; i++)
            {
                totalWeight += Mathf.Max(WheelConfig.MIN_SLICE_WEIGHT, currentSlices[i].Weight);
            }

            if (totalWeight <= 0)
            {
                return Random.Range(0, currentSlices.Count);
            }

            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;

            for (int i = 0; i < currentSlices.Count; i++)
            {
                cumulative += Mathf.Max(WheelConfig.MIN_SLICE_WEIGHT, currentSlices[i].Weight);

                if (roll < cumulative)
                {
                    return i;
                }
            }

            return currentSlices.Count - 1;
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

        private int GetSpinTypeMultiplier(SpinType spinType)
        {
            switch (spinType)
            {
                case SpinType.Golden:
                    return RewardConfig.GOLDEN_SPIN_REWARD_MULTIPLIER;
                case SpinType.Silver:
                    return RewardConfig.SILVER_SPIN_REWARD_MULTIPLIER;
                default:
                    return RewardConfig.NORMAL_SPIN_REWARD_MULTIPLIER;
            }
        }
    }
}
