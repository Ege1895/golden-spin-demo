using NUnit.Framework;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.ZoneModule;

namespace GoldenSpin.Tests.ZoneModule
{
    [TestFixture]
    public class ZoneControllerTests
    {
        private ZoneController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new ZoneController();
            _controller.Initialize();
        }

        [Test]
        public void Initialize_SetsCurrentZoneToStartingZone()
        {
            Assert.AreEqual(ZoneConfig.STARTING_ZONE_NUMBER, _controller.CurrentZone);
        }

        [Test]
        public void AdvanceZone_IncrementsCurrentZone()
        {
            _controller.AdvanceZone();
            Assert.AreEqual(2, _controller.CurrentZone);
        }

        [Test]
        public void AdvanceZone_UpdatesCurrentZoneDTO()
        {
            _controller.AdvanceZone();
            Assert.AreEqual(2, _controller.CurrentZoneDTO.ZoneNumber);
        }

        [Test]
        public void ResetZone_ResetsToStartingZone()
        {
            _controller.AdvanceZone();
            _controller.AdvanceZone();
            _controller.ResetZone();
            Assert.AreEqual(ZoneConfig.STARTING_ZONE_NUMBER, _controller.CurrentZone);
        }

        [TestCase(1, ZoneType.Normal)]
        [TestCase(4, ZoneType.Normal)]
        [TestCase(5, ZoneType.Safe)]
        [TestCase(10, ZoneType.Safe)]
        [TestCase(25, ZoneType.Safe)]
        [TestCase(30, ZoneType.Super)]
        public void GetZoneDTO_ReturnsCorrectZoneType(int zoneNumber, ZoneType expected)
        {
            Assert.AreEqual(expected, _controller.GetZoneDTO(zoneNumber).Type);
        }

        [TestCase(1, SpinType.Normal)]
        [TestCase(5, SpinType.Silver)]
        [TestCase(30, SpinType.Golden)]
        public void GetZoneDTO_ReturnsCorrectSpinType(int zoneNumber, SpinType expected)
        {
            Assert.AreEqual(expected, _controller.GetZoneDTO(zoneNumber).SpinType);
        }

        [Test]
        public void SuperZone_TakesPriorityOverSafeZone()
        {
            // Zone 30 is divisible by both 5 and 30 — Super must win
            Assert.AreEqual(ZoneType.Super, _controller.GetZoneDTO(30).Type);
        }

        [Test]
        public void IsCurrentZoneSafeToLeave_ReturnsFalseOnNormalZone()
        {
            Assert.IsFalse(_controller.IsCurrentZoneSafeToLeave());
        }

        [Test]
        public void IsCurrentZoneSafeToLeave_ReturnsTrueOnSafeZone()
        {
            for (int i = 0; i < ZoneConfig.SAFE_ZONE_INTERVAL - 1; i++)
            {
                _controller.AdvanceZone();
            }
            Assert.IsTrue(_controller.IsCurrentZoneSafeToLeave());
        }

        [Test]
        public void IsCurrentZoneSafeToLeave_ReturnsFalseOnZoneAfterSafe()
        {
            for (int i = 0; i < ZoneConfig.SAFE_ZONE_INTERVAL; i++)
            {
                _controller.AdvanceZone();
            }
            Assert.IsFalse(_controller.IsCurrentZoneSafeToLeave());
        }

        [Test]
        public void IsCurrentZoneSafeToLeave_ReturnsTrueOnSuperZone()
        {
            for (int i = 0; i < ZoneConfig.SUPER_ZONE_INTERVAL - 1; i++)
            {
                _controller.AdvanceZone();
            }
            Assert.IsTrue(_controller.IsCurrentZoneSafeToLeave());
        }

        [Test]
        public void GetCurrentZoneMultiplier_ScalesWithZoneNumber()
        {
            _controller.AdvanceZone(); // zone 2
            _controller.AdvanceZone(); // zone 3
            Assert.AreEqual(3 * RewardConfig.ZONE_MULTIPLIER_INCREMENT, _controller.GetCurrentZoneMultiplier());
        }

        [Test]
        public void GetCurrentZoneMultiplier_ReturnsGoldenMultiplierOnSuperZone()
        {
            for (int i = 0; i < ZoneConfig.SUPER_ZONE_INTERVAL - 1; i++)
            {
                _controller.AdvanceZone();
            }
            Assert.AreEqual(RewardConfig.GOLDEN_ZONE_MULTIPLIER, _controller.GetCurrentZoneMultiplier());
        }

        [Test]
        public void OnZoneChanged_FiresOnAdvanceZone()
        {
            ZoneDTO received = null;
            _controller.OnZoneChanged += dto => received = dto;
            _controller.AdvanceZone();
            Assert.AreEqual(2, received?.ZoneNumber);
        }

        [Test]
        public void OnZoneChanged_FiresOnResetZone()
        {
            _controller.AdvanceZone();
            ZoneDTO received = null;
            _controller.OnZoneChanged += dto => received = dto;
            _controller.ResetZone();
            Assert.AreEqual(ZoneConfig.STARTING_ZONE_NUMBER, received?.ZoneNumber);
        }
    }
}
