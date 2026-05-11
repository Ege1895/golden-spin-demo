using System;
using System.Collections.Generic;
using Zenject;
using GoldenSpin.CoreModule;

namespace GoldenSpin.RewardModule
{
    public class RewardController : IRewardController, IInitializable
    {
        private readonly List<RewardDTO> collectedRewards = new List<RewardDTO>();

        public IReadOnlyList<RewardDTO> CollectedRewards => collectedRewards;

        public event Action<RewardDTO>          OnRewardAdded;
        public event Action<CollectedRewardDTO> OnRewardsCleared;
        public event Action<CollectedRewardDTO> OnRewardsCollected;

        public void Initialize()
        {
            collectedRewards.Clear();
        }

        public void AddReward(RewardDTO reward)
        {
            collectedRewards.Add(reward);
            OnRewardAdded?.Invoke(reward);
        }

        public void ClearRewards()
        {
            CollectedRewardDTO snapshot = GetSnapshot();
            collectedRewards.Clear();
            OnRewardsCleared?.Invoke(snapshot);
        }

        public void CollectRewards()
        {
            CollectedRewardDTO snapshot = GetSnapshot();
            collectedRewards.Clear();
            OnRewardsCollected?.Invoke(snapshot);
        }

        public CollectedRewardDTO GetSnapshot()
        {
            CollectedRewardDTO dto = new CollectedRewardDTO();
            dto.Rewards.AddRange(collectedRewards);
            return dto;
        }
    }
}
