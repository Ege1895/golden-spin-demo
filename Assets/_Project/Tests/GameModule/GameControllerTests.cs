using System.Reflection;
using NUnit.Framework;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.GameModule;
using GoldenSpin.Tests.TestDoubles;

namespace GoldenSpin.Tests.GameModule
{
    [TestFixture]
    public class GameControllerTests
    {
        private GameController _controller;
        private ZoneControllerTestDouble _zone;
        private WheelControllerTestDouble _wheel;
        private RewardControllerTestDouble _reward;
        private CurrencyControllerTestDouble _currency;

        [SetUp]
        public void SetUp()
        {
            _zone = new ZoneControllerTestDouble();
            _wheel = new WheelControllerTestDouble();
            _reward = new RewardControllerTestDouble();
            _currency = new CurrencyControllerTestDouble(100);

            _controller = new GameController();
            Inject("zoneController", _zone);
            Inject("wheelController", _wheel);
            Inject("rewardController", _reward);
            Inject("currencyController", _currency);
            _controller.Initialize();
        }

        [Test]
        public void Initialize_SetsStateToIdle()
        {
            Assert.AreEqual(GameState.Idle, _controller.CurrentState);
        }

        [Test]
        public void Initialize_ResetsZone()
        {
            Assert.AreEqual(1, _zone.ResetZoneCallCount);
        }

        [Test]
        public void Initialize_ClearsRewards()
        {
            Assert.IsTrue(_reward.ClearRewardsCalled);
        }

        [Test]
        public void Initialize_PreparesWheel()
        {
            Assert.IsTrue(_wheel.PrepareWheelCalled);
        }

        [Test]
        public void RequestSpin_WhenIdle_SetsStateToSpinning()
        {
            _controller.RequestSpin();
            Assert.AreEqual(GameState.Spinning, _controller.CurrentState);
        }

        [Test]
        public void RequestSpin_WhenNotIdle_IsIgnored()
        {
            _controller.RequestSpin(); // Spinning
            _controller.RequestSpin(); // should be ignored
            Assert.AreEqual(1, _wheel.DetermineResultCallCount);
        }

        [Test]
        public void OnSpinAnimationCompleted_WithBomb_SetsGameOver()
        {
            _controller.RequestSpin();
            _controller.OnSpinAnimationCompleted(new WheelSliceDTO { ContentType = SliceContentType.Bomb });
            Assert.AreEqual(GameState.GameOver, _controller.CurrentState);
        }

        [Test]
        public void OnSpinAnimationCompleted_WithReward_SetsSpinResult()
        {
            _controller.RequestSpin();
            _controller.OnSpinAnimationCompleted(new WheelSliceDTO
            {
                ContentType = SliceContentType.Reward,
                Reward = new RewardDTO { Amount = 10 }
            });
            Assert.AreEqual(GameState.SpinResult, _controller.CurrentState);
        }

        [Test]
        public void OnSpinAnimationCompleted_WhenNotSpinning_IsIgnored()
        {
            _controller.OnSpinAnimationCompleted(new WheelSliceDTO { ContentType = SliceContentType.Reward });
            Assert.AreEqual(GameState.Idle, _controller.CurrentState);
        }

        [Test]
        public void OnSpinResultClosed_AdvancesZone()
        {
            SpinAndGetReward();
            _controller.OnSpinResultClosed();
            Assert.AreEqual(1, _zone.AdvanceZoneCallCount);
        }

        [Test]
        public void OnSpinResultClosed_ReturnsToIdleBelowMaxZone()
        {
            _zone.CurrentZone = 1;
            SpinAndGetReward();
            _controller.OnSpinResultClosed();
            Assert.AreEqual(GameState.Idle, _controller.CurrentState);
        }

        [Test]
        public void OnSpinResultClosed_AutoCollectsAtMaxZone()
        {
            _zone.CurrentZone = ZoneConfig.MAX_ZONE_NUMBER;
            SpinAndGetReward();
            _controller.OnSpinResultClosed();
            Assert.AreEqual(GameState.Collecting, _controller.CurrentState);
            Assert.IsTrue(_reward.CollectRewardsCalled);
        }

        [Test]
        public void RequestCollect_AtSafeZone_SetsStateToCollecting()
        {
            _zone.SafeToLeave = true;
            _controller.RequestCollect();
            Assert.AreEqual(GameState.Collecting, _controller.CurrentState);
            Assert.IsTrue(_reward.CollectRewardsCalled);
        }

        [Test]
        public void RequestCollect_AtNormalZone_IsRejected()
        {
            _zone.SafeToLeave = false;
            _controller.RequestCollect();
            Assert.AreEqual(GameState.Idle, _controller.CurrentState);
            Assert.IsFalse(_reward.CollectRewardsCalled);
        }

        [Test]
        public void RequestCollect_WhenNotIdle_IsRejected()
        {
            _zone.SafeToLeave = true;
            _controller.RequestSpin();
            _controller.RequestCollect();
            Assert.AreEqual(GameState.Spinning, _controller.CurrentState);
        }

        [Test]
        public void RestartGame_PassesThroughRestartingState()
        {
            GameState captured = GameState.Idle;
            _controller.OnStateChanged += s => { if (s == GameState.Restarting) captured = s; };
            _controller.RestartGame();
            Assert.AreEqual(GameState.Restarting, captured);
        }

        [Test]
        public void RestartGame_EndsInIdleState()
        {
            _controller.RestartGame();
            Assert.AreEqual(GameState.Idle, _controller.CurrentState);
        }

        [Test]
        public void ReviveWithCurrency_WhenGameOverAndAffordable_EntersIdle()
        {
            TriggerGameOver();
            _controller.ReviveWithCurrency();
            Assert.AreEqual(GameState.Idle, _controller.CurrentState);
        }

        [Test]
        public void ReviveWithCurrency_WhenGameOverAndAffordable_SpendsReviveCost()
        {
            int initial = _currency.CurrentAmount;
            TriggerGameOver();
            _controller.ReviveWithCurrency();
            Assert.AreEqual(initial - CurrencyConfig.REVIVE_COST, _currency.CurrentAmount);
        }

        [Test]
        public void ReviveWithCurrency_WhenGameOverAndAffordable_RestoresSnapshot()
        {
            TriggerGameOver();
            _controller.ReviveWithCurrency();
            Assert.IsTrue(_reward.RestoreLastSnapshotCalled);
        }

        [Test]
        public void ReviveWithCurrency_WhenInsufficientCurrency_StaysInGameOver()
        {
            Inject("currencyController", new CurrencyControllerTestDouble(0));
            TriggerGameOver();
            _controller.ReviveWithCurrency();
            Assert.AreEqual(GameState.GameOver, _controller.CurrentState);
        }

        [Test]
        public void ReviveWithCurrency_WhenNotGameOver_IsIgnored()
        {
            int initial = _currency.CurrentAmount;
            _controller.ReviveWithCurrency();
            Assert.AreEqual(GameState.Idle, _controller.CurrentState);
            Assert.AreEqual(initial, _currency.CurrentAmount);
        }

        private void SpinAndGetReward()
        {
            _controller.RequestSpin();
            _controller.OnSpinAnimationCompleted(new WheelSliceDTO
            {
                ContentType = SliceContentType.Reward,
                Reward = new RewardDTO { Amount = 10 }
            });
        }

        private void TriggerGameOver()
        {
            _controller.RequestSpin();
            _controller.OnSpinAnimationCompleted(new WheelSliceDTO { ContentType = SliceContentType.Bomb });
        }

        private void Inject(string fieldName, object value)
        {
            _controller.GetType()
                .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_controller, value);
        }
    }
}
