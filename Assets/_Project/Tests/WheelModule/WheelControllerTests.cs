using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.WheelModule;
using GoldenSpin.Tests.TestDoubles;

namespace GoldenSpin.Tests.WheelModule
{
    [TestFixture]
    public class WheelControllerTests
    {
        private WheelController _controller;
        private RewardControllerTestDouble _reward;
        private ZoneConfigSO _zoneConfigSO;

        [SetUp]
        public void SetUp()
        {
            _reward = new RewardControllerTestDouble();
            _zoneConfigSO = BuildZoneConfigSO();

            _controller = new WheelController();
            Inject("rewardController", _reward);
            Inject("zoneConfigSO", _zoneConfigSO);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_zoneConfigSO);
        }

        [Test]
        public void PrepareWheel_PopulatesCurrentSlices()
        {
            _controller.PrepareWheel(SpinType.Normal, 1);
            Assert.Greater(_controller.CurrentSlices.Count, 0);
        }

        [Test]
        public void PrepareWheel_SliceCountMatchesConfig()
        {
            _controller.PrepareWheel(SpinType.Normal, 1);
            Assert.AreEqual(_zoneConfigSO.NormalWheelConfig.Slices.Count, _controller.CurrentSlices.Count);
        }

        [Test]
        public void PrepareWheel_ClearsOldSlicesBeforePopulating()
        {
            _controller.PrepareWheel(SpinType.Normal, 1);
            int first = _controller.CurrentSlices.Count;
            _controller.PrepareWheel(SpinType.Normal, 1);
            Assert.AreEqual(first, _controller.CurrentSlices.Count);
        }

        [Test]
        public void PrepareWheel_FiresOnWheelPrepared()
        {
            SpinType received = SpinType.Normal;
            _controller.OnWheelPrepared += t => received = t;
            _controller.PrepareWheel(SpinType.Golden, 1);
            Assert.AreEqual(SpinType.Golden, received);
        }

        [Test]
        public void PrepareWheel_AppliesZoneMultiplierToRewards()
        {
            int baseAmount = _zoneConfigSO.NormalWheelConfig.Slices[0].Reward.BaseAmount;
            _controller.PrepareWheel(SpinType.Normal, 3);
            WheelSliceDTO first = _controller.CurrentSlices[0];
            Assert.AreEqual(baseAmount * 3, first.Reward.Amount);
        }

        [Test]
        public void PrepareWheel_SilverConfig_HasNoBomb()
        {
            _controller.PrepareWheel(SpinType.Silver, 1);
            foreach (WheelSliceDTO slice in _controller.CurrentSlices)
                Assert.AreNotEqual(SliceContentType.Bomb, slice.ContentType);
        }

        [Test]
        public void PrepareWheel_GoldenConfig_HasNoBomb()
        {
            _controller.PrepareWheel(SpinType.Golden, 1);
            foreach (WheelSliceDTO slice in _controller.CurrentSlices)
                Assert.AreNotEqual(SliceContentType.Bomb, slice.ContentType);
        }

        [Test]
        public void PrepareWheel_NormalConfig_ContainsBomb()
        {
            _controller.PrepareWheel(SpinType.Normal, 1);
            bool hasBomb = false;
            foreach (WheelSliceDTO slice in _controller.CurrentSlices)
                if (slice.ContentType == SliceContentType.Bomb) hasBomb = true;
            Assert.IsTrue(hasBomb);
        }

        [Test]
        public void DetermineResultSliceIndex_ReturnsIndexInRange()
        {
            _controller.PrepareWheel(SpinType.Normal, 1);
            int index = _controller.DetermineResultSliceIndex();
            Assert.GreaterOrEqual(index, 0);
            Assert.Less(index, _controller.CurrentSlices.Count);
        }

        [Test]
        public void DetermineResultSliceIndex_FiresOnSpinStarted()
        {
            _controller.PrepareWheel(SpinType.Normal, 1);
            int received = -1;
            _controller.OnSpinStarted += i => received = i;
            int index = _controller.DetermineResultSliceIndex();
            Assert.AreEqual(index, received);
        }

        [Test]
        public void ProcessSpinResult_WithReward_AddsToRewardController()
        {
            _controller.ProcessSpinResult(new WheelSliceDTO
            {
                ContentType = SliceContentType.Reward,
                Reward = new RewardDTO { Amount = 50 }
            });
            Assert.AreEqual(1, _reward.AddRewardCallCount);
        }

        [Test]
        public void ProcessSpinResult_WithBomb_ClearsRewards()
        {
            _controller.ProcessSpinResult(new WheelSliceDTO { ContentType = SliceContentType.Bomb });
            Assert.IsTrue(_reward.ClearRewardsCalled);
        }

        [Test]
        public void ProcessSpinResult_WithBomb_DoesNotAddReward()
        {
            _controller.ProcessSpinResult(new WheelSliceDTO { ContentType = SliceContentType.Bomb });
            Assert.AreEqual(0, _reward.AddRewardCallCount);
        }

        [Test]
        public void ProcessSpinResult_FiresOnSpinCompleted()
        {
            WheelSliceDTO received = null;
            _controller.OnSpinCompleted += s => received = s;
            var slice = new WheelSliceDTO { ContentType = SliceContentType.Reward, Reward = new RewardDTO { Amount = 10 } };
            _controller.ProcessSpinResult(slice);
            Assert.AreEqual(10, received?.Reward.Amount);
        }

        [Test]
        public void GetTargetAngleForSlice_ReturnsZeroForFirstSlice()
        {
            Assert.AreEqual(0f, _controller.GetTargetAngleForSlice(0));
        }

        [Test]
        public void GetTargetAngleForSlice_ReturnsCorrectAngle()
        {
            Assert.AreEqual(WheelConfig.SLICE_ANGLE, _controller.GetTargetAngleForSlice(1));
        }

        private static ZoneConfigSO BuildZoneConfigSO()
        {
            ZoneConfigSO so = ScriptableObject.CreateInstance<ZoneConfigSO>();
            so.NormalWheelConfig = BuildWheelConfig(SpinType.Normal, hasBomb: true);
            so.SilverWheelConfig = BuildWheelConfig(SpinType.Silver, hasBomb: false);
            so.GoldenWheelConfig = BuildWheelConfig(SpinType.Golden, hasBomb: false);
            return so;
        }

        private static WheelConfigSO BuildWheelConfig(SpinType spinType, bool hasBomb)
        {
            WheelConfigSO config = ScriptableObject.CreateInstance<WheelConfigSO>();
            config.SpinType = spinType;
            config.Slices = new List<WheelSliceSO>();

            for (int i = 0; i < 7; i++)
            {
                WheelSliceSO slice = ScriptableObject.CreateInstance<WheelSliceSO>();
                slice.ContentType = SliceContentType.Reward;
                RewardSO reward = ScriptableObject.CreateInstance<RewardSO>();
                reward.BaseAmount = 10 * (i + 1);
                slice.Reward = reward;
                config.Slices.Add(slice);
            }

            if (hasBomb)
            {
                WheelSliceSO bomb = ScriptableObject.CreateInstance<WheelSliceSO>();
                bomb.ContentType = SliceContentType.Bomb;
                config.Slices.Add(bomb);
            }

            return config;
        }

        private void Inject(string fieldName, object value)
        {
            _controller.GetType()
                .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_controller, value);
        }
    }
}
