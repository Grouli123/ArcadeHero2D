using System;

namespace ArcadeHero2D.Minigame
{
    public static class CoinRegistry
    {
        public static int Live { get; private set; }
        public static event Action<int> OnLiveChanged;
        public static event Action OnAllGone;

        public static void Reset()
        {
            Live = 0;
            OnLiveChanged?.Invoke(Live);
            OnAllGone?.Invoke();
        }

        public static void Register(Coin c)
        {
            Live++;
            OnLiveChanged?.Invoke(Live);
        }

        public static void Unregister(Coin c)
        {
            if (Live > 0) Live--;
            OnLiveChanged?.Invoke(Live);
            if (Live == 0) OnAllGone?.Invoke();
        }
    }
}