using System;

namespace GoldenSpin.CoreModule
{
    public interface ICurrencyController
    {
        int CurrentAmount { get; }

        event Action<int> OnCurrencyChanged;

        bool CanAfford(int amount);

        bool TrySpend(int amount);

        void Add(int amount);
    }
}
