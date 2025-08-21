using System;
using ArcadeHero2D.Domain.Contracts;

namespace ArcadeHero2D.Services.Currency
{
    public sealed class CurrencyService : ICurrencyService
    {
        public int Soft { get; private set; }
        public event Action<int> OnChanged;

        public void Add(int value)
        {
            Soft += Math.Max(0, value);
            OnChanged?.Invoke(Soft);
        }

        public bool TrySpend(int value)
        {
            if (value <= 0) return true;
            if (Soft < value) return false;
            Soft -= value;
            OnChanged?.Invoke(Soft);
            return true;
        }
    }
}