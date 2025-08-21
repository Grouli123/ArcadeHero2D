using System;
using ArcadeHero2D.Data.Units;
using ArcadeHero2D.Domain.Contracts;

namespace ArcadeHero2D.Services.Stats
{
    public sealed class StatsService : IStatsService
    {
        public int Attack { get; private set; }
        public float AttackSpeed { get; private set; }
        public int MaxHP { get; private set; }

        public event Action OnChanged;

        public StatsService(UnitStats hero)
        {
            Attack = hero.attack;
            AttackSpeed = hero.attackRate;
            MaxHP = hero.maxHP;
            OnChanged?.Invoke();
        }

        public void AddAttack(int delta)       { Attack += delta; OnChanged?.Invoke(); }
        public void AddAttackSpeed(float delta){ AttackSpeed += delta; OnChanged?.Invoke(); }
        public void AddMaxHP(int delta, bool healToFull = true)
        {
            MaxHP += delta;
            OnChanged?.Invoke();
        }
    }
}