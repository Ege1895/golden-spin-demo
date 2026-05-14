using System;
using System.Collections.Generic;
using GoldenSpin.CoreModule;

namespace GoldenSpin.Tests.TestDoubles
{
    public class RewardControllerTestDouble : IRewardController
    {
        private readonly List<RewardDTO> rewards = new List<RewardDTO>();

        public IReadOnlyList<RewardDTO> CollectedRewards => rewards;
        public bool ClearRewardsCalled { get; private set; }
        public bool CollectRewardsCalled { get; private set; }
        public bool RestoreLastSnapshotCalled { get; private set; }
        public int AddRewardCallCount { get; private set; }

        public event Action<RewardDTO> OnRewardAdded;
        public event Action<CollectedRewardDTO> OnRewardsCleared;
        public event Action<CollectedRewardDTO> OnRewardsCollected;

        public void AddReward(RewardDTO reward)
        {
            AddRewardCallCount++;
            rewards.Add(reward);
            OnRewardAdded?.Invoke(reward);
        }

        public void ClearRewards()
        {
            ClearRewardsCalled = true;
            CollectedRewardDTO snapshot = GetSnapshot();
            rewards.Clear();
            OnRewardsCleared?.Invoke(snapshot);
        }

        public void CollectRewards()
        {
            CollectRewardsCalled = true;
            CollectedRewardDTO snapshot = GetSnapshot();
            rewards.Clear();
            OnRewardsCollected?.Invoke(snapshot);
        }

        public void RestoreLastSnapshot() => RestoreLastSnapshotCalled = true;

        public CollectedRewardDTO GetSnapshot()
        {
            CollectedRewardDTO dto = new CollectedRewardDTO();
            dto.Rewards.AddRange(rewards);
            return dto;
        }
    }
}
