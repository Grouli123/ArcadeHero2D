using ArcadeHero2D.Domain.Base;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyController : UnitBase
    {
        [SerializeField] private EnemyMover mover;
        [SerializeField] private EnemyAttack attack;

        private IEnemyResponder[] _responders;

        public void Init(Transform hero)
        {
            mover.SetTarget(hero);
            mover.SetLaneY(hero.position.y);
            attack.SetTarget(hero);

            _responders = GetComponents<IEnemyResponder>();
            foreach (var r in _responders) r.Init(hero);
        }
    }
}