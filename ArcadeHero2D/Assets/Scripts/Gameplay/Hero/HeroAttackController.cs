using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Contracts;
using ArcadeHero2D.Rendering;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Hero
{
    public sealed class HeroAttackController : MonoBehaviour, IAttacker
    {
        [SerializeField] private Projectiles.ProjectileBase projectilePrefab;
        [SerializeField] private Transform firePoint;

        IStatsService _stats;
        UnitAnimationController _anim;
        float _cd;

        public bool AttackEnabled { get; set; } = true;

        public int  AttackDamage => _stats != null ? _stats.Attack      : 1;
        public float AttackRate  => _stats != null ? _stats.AttackSpeed : 1f;
        public bool CanAttack    => _cd <= 0f && AttackEnabled;

        void Awake()
        {
            _stats = ServiceLocator.Get<IStatsService>();
            _anim  = GetComponentInParent<UnitAnimationController>();
        }

        void Update()
        {
            if (_cd > 0f) _cd -= Time.deltaTime;
        }

        public void TryAttack(Transform target)
        {
            if (!CanAttack || target == null) return;
            _cd = 1f / Mathf.Max(0.01f, AttackRate);

            if (_anim) _anim.RequestAttack();

            var p = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            p.Launch(firePoint.position, target.position, AttackDamage);
        }
    }
}