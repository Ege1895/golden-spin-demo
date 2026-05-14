using System;
using GoldenSpin.CoreModule;

namespace GoldenSpin.Tests.TestDoubles
{
    public class CurrencyControllerTestDouble : ICurrencyController
    {
        public int CurrentAmount { get; private set; }
        public int EventFireCount { get; private set; }

        public event Action<int> OnCurrencyChanged;

        public CurrencyControllerTestDouble(int startingAmount = 100)
        {
            CurrentAmount = startingAmount;
        }

        public bool CanAfford(int amount) => CurrentAmount >= amount;

        public bool TrySpend(int amount)
        {
            if (!CanAfford(amount)) return false;
            CurrentAmount -= amount;
            EventFireCount++;
            OnCurrencyChanged?.Invoke(CurrentAmount);
            return true;
        }

        public void Add(int amount)
        {
            CurrentAmount += amount;
            EventFireCount++;
            OnCurrencyChanged?.Invoke(CurrentAmount);
        }
    }
}
