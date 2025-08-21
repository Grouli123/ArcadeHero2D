using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Hero
{
    public sealed class HeroAttackController : MonoBehaviour, IAttacker
    {
        [SerializeField] Projectiles.ProjectileBase projectilePrefab;
        [SerializeField] Transform firePoint;

        IStatsService _stats;
        float _cd;

        public int AttackDamage => _stats.Attack;
        public float AttackRate  => _stats.AttackSpeed;
        public bool CanAttack    => _cd <= 0f;

        void Awake() => _stats = ServiceLocator.Get<IStatsService>();
        void Update() { if (_cd > 0) _cd -= Time.deltaTime; }

        public void TryAttack(Transform target)
        {
            if (!CanAttack || target == null) return;
            _cd = 1f / Mathf.Max(0.01f, AttackRate);

            var p = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            p.Launch(firePoint.position, target.position, AttackDamage);
        }
    }
}