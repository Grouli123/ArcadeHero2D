using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyMeleeResponder : MonoBehaviour, IEnemyResponder
    {
        [SerializeField] private float stepDistance = 0.4f;
        [SerializeField] private float meleeRange   = 1.05f;
        [SerializeField] private int   damagePerHit = 2;
        [SerializeField] private float hitCooldown  = 0.6f;
        [SerializeField] private float minSpacing   = 0.8f;

        public EnemyLaneController lane;

        private Transform _hero;
        private float _cd;
        private EnemyController _self;
        private SlotMover _slot;

        public void Init(Transform hero)
        {
            _hero  = hero;
            _self  = GetComponent<EnemyController>();
            _slot  = GetComponent<SlotMover>();
        }

        public void OnHeroAttacked()
        {
            if (_hero == null || _slot == null) return;
            if (_cd > 0f) return;
            if (!_slot.InSlot) return; 

            float dx  = _hero.position.x - transform.position.x;
            float adx = Mathf.Abs(dx);

            if (adx > meleeRange)
            {
                float dir = Mathf.Sign(dx);
                float desiredX = transform.position.x + dir * stepDistance;
                if (lane != null && _self != null)
                    desiredX = lane.GetAllowedStepX(_self, desiredX, minSpacing);
                _slot.SetTargetX(desiredX);
            }
            else
            {
                if (_hero.TryGetComponent<IDamageable>(out var d))
                    d.TakeDamage(damagePerHit);
                _cd = hitCooldown;
            }
        }

        private void Update()
        {
            if (_cd > 0f) _cd -= Time.deltaTime;
        }
    }
}