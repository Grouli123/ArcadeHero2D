using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Base;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Hero
{
    public sealed class HeroController : UnitBase
    {
        [SerializeField] HeroMover mover;
        [SerializeField] HeroTargeting targeting;
        [SerializeField] HeroAttackController attack;

        IStatsService _stats;

        protected override void Awake()
        {
            base.Awake();
            _stats = ServiceLocator.Get<IStatsService>();
            // Подгоняем HP под статы:
            _health.IncreaseMax(_stats.MaxHP - _health.Max, healToFull: true);
            _stats.OnChanged += ApplyStatsToHealth;
        }

        void ApplyStatsToHealth()
        {
            // если MaxHP вырос — обновим
            int delta = _stats.MaxHP - _health.Max;
            if (delta != 0) _health.IncreaseMax(delta, true);
        }

        void Update()
        {
            if (targeting.HasTarget)
            {
                mover.Stop();
                attack.TryAttack(targeting.CurrentTarget);
            }
            else
            {
                mover.Move(Vector2.right, mover.MoveSpeed);
            }
        }
    }
}