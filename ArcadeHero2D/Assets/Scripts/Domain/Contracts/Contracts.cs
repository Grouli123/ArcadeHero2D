using System;
using UnityEngine;

namespace ArcadeHero2D.Domain.Contracts
{
    public interface IHealth
    {
        int Max { get; }
        int Current { get; }
        event Action<int,int> OnChanged;
        event Action OnDied;
        void Take(int amount);
        void Heal(int amount);
        void IncreaseMax(int delta, bool healToFull);
    }

    public interface IDamageable
    {
        IHealth Health { get; }
        void TakeDamage(int amount);
        bool IsAlive { get; }
    }

    public interface IMovable
    {
        void Move(Vector2 direction, float speed);
        void Stop();
    }

    public interface ITargetProvider
    {
        Transform CurrentTarget { get; }
        bool HasTarget { get; }
    }

    public interface IAttacker
    {
        int AttackDamage { get; }
        float AttackRate { get; }
        bool CanAttack { get; }
        void TryAttack(Transform target);
    }

    public interface IProjectile
    {
        void Launch(Vector2 origin, Vector2 target, int damage);
    }

    public interface ICurrencyService
    {
        int Soft { get; }
        event Action<int> OnChanged;
        void Add(int value);
        bool TrySpend(int value);
    }

    public interface IStatsService
    {
        int Attack { get; }
        float AttackSpeed { get; }
        int MaxHP { get; }
        event Action OnChanged;

        void AddAttack(int delta);
        void AddAttackSpeed(float delta);
        void AddMaxHP(int delta, bool healToFull = true);
    }

    public interface IUpgradeService
    {
        ArcadeHero2D.Data.Upgrades.UpgradeDefinition[] Roll3();
        bool TryApply(ArcadeHero2D.Data.Upgrades.UpgradeDefinition def);
    }
}