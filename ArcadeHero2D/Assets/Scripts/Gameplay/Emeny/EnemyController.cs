using ArcadeHero2D.Domain.Base;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyController : UnitBase
    {
        [SerializeField] private SlotMover slotMover;
        [SerializeField] private EnemyAttack attack;
        private IEnemyResponder[] _responders;

        public bool IsReady => slotMover == null || slotMover.InSlot;

        public void Init(Transform hero)
        {
            if (slotMover != null) slotMover.SetLaneY(hero.position.y);
            if (attack != null) attack.SetTarget(hero);
            _responders = GetComponents<IEnemyResponder>();
            foreach (var r in _responders) r.Init(hero);
        }

        public void AssignSlot(float slotX)
        {
            if (slotMover == null) return;
            slotMover.SetSlotX(slotX);
        }
    }
}