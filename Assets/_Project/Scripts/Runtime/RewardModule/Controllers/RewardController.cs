using System;
using System.Collections.Generic;
using Zenject;
using GoldenSpin.CoreModule;

namespace GoldenSpin.RewardModule
{
    public class RewardController : IRewardController, IInitializable
    {
        private readonly List<RewardDTO> collectedRewards = new List<RewardDTO>();
        private CollectedRewardDTO lastClearedSnapshot;

        public IReadOnlyList<RewardDTO> CollectedRewards => collectedRewards;

        public event Action<RewardDTO> OnRewardAdded;
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
            lastClearedSnapshot = GetSnapshot();
            collectedRewards.Clear();
            OnRewardsCleared?.Invoke(lastClearedSnapshot);
        }

        public void RestoreLastSnapshot()
        {
            if (lastClearedSnapshot == null)
            {
                return;
            }

            foreach (RewardDTO reward in lastClearedSnapshot.Rewards)
            {
                collectedRewards.Add(reward);
                OnRewardAdded?.Invoke(reward);
            }

            lastClearedSnapshot = null;
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
