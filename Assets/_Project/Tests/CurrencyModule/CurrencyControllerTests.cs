using System.Reflection;
using NUnit.Framework;
using GoldenSpin.ConfigurationModule;
using GoldenSpin.CoreModule;
using GoldenSpin.CurrencyModule;
using GoldenSpin.Tests.TestDoubles;

namespace GoldenSpin.Tests.CurrencyModule
{
    [TestFixture]
    public class CurrencyControllerTests
    {
        private CurrencyController _controller;
        private FakeSaveService _saveService;

        [SetUp]
        public void SetUp()
        {
            _saveService = new FakeSaveService();
            _controller = BuildController(_saveService);
            _controller.Initialize();
        }

        [Test]
        public void Initialize_UsesStartingAmountWhenNothingSaved()
        {
            Assert.AreEqual(CurrencyConfig.STARTING_AMOUNT, _controller.CurrentAmount);
        }

        [Test]
        public void Initialize_LoadsPreviouslySavedAmount()
        {
            FakeSaveService preloaded = new FakeSaveService();
            preloaded.SaveInt(CurrencyConfig.PREFS_KEY_GOLD, 42);
            CurrencyController controller = BuildController(preloaded);
            controller.Initialize();
            Assert.AreEqual(42, controller.CurrentAmount);
        }

        [Test]
        public void Initialize_FiresOnCurrencyChanged()
        {
            int received = -1;
            CurrencyController controller = BuildController(new FakeSaveService());
            controller.OnCurrencyChanged += a => received = a;
            controller.Initialize();
            Assert.AreEqual(CurrencyConfig.STARTING_AMOUNT, received);
        }

        [Test]
        public void CanAfford_ReturnsTrueWhenExactAmount()
        {
            Assert.IsTrue(_controller.CanAfford(_controller.CurrentAmount));
        }

        [Test]
        public void CanAfford_ReturnsFalseWhenInsufficient()
        {
            Assert.IsFalse(_controller.CanAfford(_controller.CurrentAmount + 1));
        }

        [Test]
        public void TrySpend_ReturnsTrueAndDeducts()
        {
            int initial = _controller.CurrentAmount;
            Assert.IsTrue(_controller.TrySpend(10));
            Assert.AreEqual(initial - 10, _controller.CurrentAmount);
        }

        [Test]
        public void TrySpend_ReturnsFalseAndDoesNotDeductWhenInsufficient()
        {
            int initial = _controller.CurrentAmount;
            Assert.IsFalse(_controller.TrySpend(initial + 1));
            Assert.AreEqual(initial, _controller.CurrentAmount);
        }

        [Test]
        public void TrySpend_SucceedsOnExactBalance()
        {
            Assert.IsTrue(_controller.TrySpend(_controller.CurrentAmount));
            Assert.AreEqual(0, _controller.CurrentAmount);
        }

        [Test]
        public void TrySpend_FiresOnCurrencyChanged()
        {
            int received = -1;
            _controller.OnCurrencyChanged += a => received = a;
            _controller.TrySpend(10);
            Assert.AreEqual(_controller.CurrentAmount, received);
        }

        [Test]
        public void TrySpend_DoesNotFireEventOnFailure()
        {
            bool fired = false;
            _controller.OnCurrencyChanged += _ => fired = true;
            _controller.TrySpend(_controller.CurrentAmount + 1);
            Assert.IsFalse(fired);
        }

        [Test]
        public void Add_IncreasesCurrentAmount()
        {
            int initial = _controller.CurrentAmount;
            _controller.Add(25);
            Assert.AreEqual(initial + 25, _controller.CurrentAmount);
        }

        [Test]
        public void Add_FiresOnCurrencyChanged()
        {
            int received = -1;
            _controller.OnCurrencyChanged += a => received = a;
            _controller.Add(10);
            Assert.AreEqual(_controller.CurrentAmount, received);
        }

        [Test]
        public void TrySpend_PersistsAmountThroughSaveService()
        {
            _controller.TrySpend(10);
            Assert.AreEqual(_controller.CurrentAmount, _saveService.LoadInt(CurrencyConfig.PREFS_KEY_GOLD, 0));
        }

        [Test]
        public void Add_PersistsAmountThroughSaveService()
        {
            _controller.Add(50);
            Assert.AreEqual(_controller.CurrentAmount, _saveService.LoadInt(CurrencyConfig.PREFS_KEY_GOLD, 0));
        }

        private static CurrencyController BuildController(ISaveService saveService)
        {
            CurrencyController controller = new CurrencyController();
            typeof(CurrencyController)
                .GetField("saveService", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(controller, saveService);
            return controller;
        }
    }
}
