using System;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Domain.Base
{
    public sealed class Health : IHealth
    {
        public int Max { get; private set; }
        public int Current { get; private set; }

        public event Action<int,int> OnChanged;
        public event Action OnDied;

        public Health(int max)
        {
            Max = Mathf.Max(1, max);
            Current = Max;
            OnChanged?.Invoke(Current, Max);
        }

        public void Take(int amount)
        {
            if (Current <= 0) return;
            Current = Mathf.Max(0, Current - Mathf.Max(0, amount));
            OnChanged?.Invoke(Current, Max);
            if (Current == 0) OnDied?.Invoke();
        }

        public void Heal(int amount)
        {
            if (Current <= 0) return;
            Current = Mathf.Min(Max, Current + Mathf.Max(0, amount));
            OnChanged?.Invoke(Current, Max);
        }

        public void IncreaseMax(int delta, bool healToFull)
        {
            Max = Mathf.Max(1, Max + delta);
            if (healToFull) Current = Max;
            OnChanged?.Invoke(Current, Max);
        }
    }
}