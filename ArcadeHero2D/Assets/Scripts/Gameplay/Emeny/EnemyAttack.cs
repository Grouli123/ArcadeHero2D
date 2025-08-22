using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private int damagePerHit = 2;
        [SerializeField] private float hitInterval = 0.6f;

        private Transform _target;
        private float _cd;

        public void SetTarget(Transform t) => _target = t;

        private void Update()
        {
            if (_target == null) return;
            if (_cd > 0f) { _cd -= Time.deltaTime; return; }

            float dx = Mathf.Abs(_target.position.x - transform.position.x);
            if (dx <= 1.05f)
            {
                if (_target.TryGetComponent<IDamageable>(out var dmg))
                    dmg.TakeDamage(damagePerHit);
                _cd = hitInterval;
            }
        }
    }
}