using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Domain.Base
{
    public abstract class UnitBase : MonoBehaviour, IDamageable
    {
        [SerializeField] protected int startMaxHP = 10;
        protected Health _health;

        public IHealth Health => _health;
        public bool IsAlive => _health != null && _health.Current > 0;

        protected virtual void Awake()
        {
            _health = new Health(startMaxHP);
            _health.OnDied += OnDied;
        }

        public virtual void TakeDamage(int amount) => _health.Take(amount);

        // Важно: ничего не удаляем! Смерть и удаление управляются UnitDeathHandler/RewardOnDeath.
        protected virtual void OnDied()
        {
            // intentionally empty
        }
    }
}