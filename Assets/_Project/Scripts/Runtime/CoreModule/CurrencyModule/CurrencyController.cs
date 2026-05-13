using System;
using Zenject;
using GoldenSpin.CoreModule;
using GoldenSpin.ConfigurationModule;

namespace GoldenSpin.CurrencyModule
{
    public class CurrencyController : ICurrencyController, IInitializable
    {
        private int currentAmount;

        public int CurrentAmount => currentAmount;

        public event Action<int> OnCurrencyChanged;

        public void Initialize()
        {
            currentAmount = CurrencyConfig.STARTING_AMOUNT;
            OnCurrencyChanged?.Invoke(currentAmount);
        }

        public bool CanAfford(int amount)
        {
            return currentAmount >= amount;
        }

        public bool TrySpend(int amount)
        {
            if (!CanAfford(amount))
            {
                return false;
            }

            currentAmount -= amount;
            OnCurrencyChanged?.Invoke(currentAmount);
            return true;
        }

        public void Add(int amount)
        {
            currentAmount += amount;
            OnCurrencyChanged?.Invoke(currentAmount);
        }
    }
}
