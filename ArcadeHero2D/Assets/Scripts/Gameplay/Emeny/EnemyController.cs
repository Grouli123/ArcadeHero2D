using ArcadeHero2D.Domain.Base;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyController : UnitBase
    {
        [SerializeField] EnemyMover mover;
        [SerializeField] EnemyAttack attack;

        public void Init(Transform hero)
        {
            mover.SetTarget(hero);
            mover.SetLaneY(hero.position.y);  // <-- фиксируем высоту движения
            attack.SetTarget(hero);
        }
    }
}