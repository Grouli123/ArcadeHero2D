using System;
using ArcadeHero2D.Domain.Contracts;

namespace ArcadeHero2D.Services.CupBank
{
    public sealed class CupBankService : ICupBankService
    {
        public int BufferedCoins { get; private set; }
        public event Action<int> OnBufferedChanged;

        public void AddBuffered(int value)
        {
            if (value <= 0) return;
            BufferedCoins += value;
            OnBufferedChanged?.Invoke(BufferedCoins);
        }

        public int TakeAll()
        {
            int v = BufferedCoins;
            BufferedCoins = 0;
            OnBufferedChanged?.Invoke(BufferedCoins);
            return v;
        }

        public void FlushToGlobal(ICurrencyService currency)
        {
            if (BufferedCoins <= 0) return;
            currency.Add(BufferedCoins);
            BufferedCoins = 0;
            OnBufferedChanged?.Invoke(BufferedCoins);
        }
    }
}