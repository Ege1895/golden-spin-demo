using System;
using System.Collections.Generic;

namespace GoldenSpin.CoreModule
{
    public interface IRewardController
    {
        IReadOnlyList<RewardDTO> CollectedRewards { get; }

        event Action<RewardDTO> OnRewardAdded;
        event Action<CollectedRewardDTO> OnRewardsCleared;
        event Action<CollectedRewardDTO> OnRewardsCollected;

        void AddReward(RewardDTO reward);
        
        void ClearRewards();
        
        void CollectRewards();

        void RestoreLastSnapshot();

        CollectedRewardDTO GetSnapshot();
    }
}
