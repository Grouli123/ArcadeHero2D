using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Base;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Hero
{
    public sealed class HeroController : UnitBase
    {
        [SerializeField] public HeroMover mover;
        [SerializeField] public HeroTargeting targeting;
        [SerializeField] public HeroAttackController attack;

        IStatsService _stats;

        protected override void Awake()
        {
            base.Awake();
            _stats = ServiceLocator.Get<IStatsService>();
            _health.IncreaseMax(_stats.MaxHP - _health.Max, true);
            _stats.OnChanged += ApplyStatsToHealth;
        }

        void ApplyStatsToHealth()
        {
            int delta = _stats.MaxHP - _health.Max;
            if (delta != 0) _health.IncreaseMax(delta, true);
        }
    }
}