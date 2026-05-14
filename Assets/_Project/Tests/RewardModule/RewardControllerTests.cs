using NUnit.Framework;
using GoldenSpin.CoreModule;
using GoldenSpin.RewardModule;

namespace GoldenSpin.Tests.RewardModule
{
    [TestFixture]
    public class RewardControllerTests
    {
        private RewardController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new RewardController();
            _controller.Initialize();
        }

        private static RewardDTO MakeReward(int amount = 10) =>
            new RewardDTO { Type = RewardType.Cash, Amount = amount };

        [Test]
        public void Initialize_StartsWithNoRewards()
        {
            Assert.AreEqual(0, _controller.CollectedRewards.Count);
        }

        [Test]
        public void AddReward_IncreasesCount()
        {
            _controller.AddReward(MakeReward());
            Assert.AreEqual(1, _controller.CollectedRewards.Count);
        }

        [Test]
        public void AddReward_StoresCorrectAmount()
        {
            _controller.AddReward(MakeReward(50));
            Assert.AreEqual(50, _controller.CollectedRewards[0].Amount);
        }

        [Test]
        public void AddReward_FiresOnRewardAdded()
        {
            RewardDTO received = null;
            _controller.OnRewardAdded += r => received = r;
            _controller.AddReward(MakeReward(99));
            Assert.AreEqual(99, received?.Amount);
        }

        [Test]
        public void ClearRewards_EmptiesCollection()
        {
            _controller.AddReward(MakeReward());
            _controller.AddReward(MakeReward());
            _controller.ClearRewards();
            Assert.AreEqual(0, _controller.CollectedRewards.Count);
        }

        [Test]
        public void ClearRewards_FiresEventWithSnapshot()
        {
            _controller.AddReward(MakeReward(10));
            _controller.AddReward(MakeReward(20));
            CollectedRewardDTO snapshot = null;
            _controller.OnRewardsCleared += s => snapshot = s;
            _controller.ClearRewards();
            Assert.AreEqual(2, snapshot?.Rewards.Count);
        }

        [Test]
        public void CollectRewards_EmptiesCollection()
        {
            _controller.AddReward(MakeReward());
            _controller.CollectRewards();
            Assert.AreEqual(0, _controller.CollectedRewards.Count);
        }

        [Test]
        public void CollectRewards_FiresOnRewardsCollectedWithSnapshot()
        {
            _controller.AddReward(MakeReward(100));
            CollectedRewardDTO snapshot = null;
            _controller.OnRewardsCollected += s => snapshot = s;
            _controller.CollectRewards();
            Assert.AreEqual(100, snapshot?.Rewards[0].Amount);
        }

        [Test]
        public void CollectRewards_DoesNotFireOnRewardsCleared()
        {
            bool fired = false;
            _controller.OnRewardsCleared += _ => fired = true;
            _controller.AddReward(MakeReward());
            _controller.CollectRewards();
            Assert.IsFalse(fired);
        }

        [Test]
        public void RestoreLastSnapshot_RestoresRewardsAfterClear()
        {
            _controller.AddReward(MakeReward(10));
            _controller.AddReward(MakeReward(20));
            _controller.ClearRewards();
            _controller.RestoreLastSnapshot();
            Assert.AreEqual(2, _controller.CollectedRewards.Count);
        }

        [Test]
        public void RestoreLastSnapshot_IsNoOpWhenNoPriorClear()
        {
            _controller.RestoreLastSnapshot();
            Assert.AreEqual(0, _controller.CollectedRewards.Count);
        }

        [Test]
        public void RestoreLastSnapshot_CanOnlyBeUsedOnce()
        {
            _controller.AddReward(MakeReward());
            _controller.ClearRewards();
            _controller.RestoreLastSnapshot();
            _controller.ClearRewards();
            _controller.RestoreLastSnapshot(); // snapshot is gone
            Assert.AreEqual(0, _controller.CollectedRewards.Count);
        }

        [Test]
        public void GetSnapshot_IsIndependentCopyOfCollection()
        {
            _controller.AddReward(MakeReward(5));
            CollectedRewardDTO snapshot = _controller.GetSnapshot();
            _controller.AddReward(MakeReward(10));
            Assert.AreEqual(1, snapshot.Rewards.Count);
        }
    }
}
