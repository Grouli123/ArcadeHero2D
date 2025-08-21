using System;

namespace ArcadeHero2D.Domain.Contracts
{
    public interface ICupBankService
    {
        int BufferedCoins { get; }
        event Action<int> OnBufferedChanged;
        void AddBuffered(int value);
        int TakeAll();
        void FlushToGlobal(ICurrencyService currency);
    }
}