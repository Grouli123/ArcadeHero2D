using ArcadeHero2D.Gameplay.Projectiles;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyArcherResponder : MonoBehaviour, IEnemyResponder
    {
        [SerializeField] private LinearProjectile arrowPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private int damagePerShot = 2;
        [SerializeField] private float shotCooldown = 0.7f;

        private Transform _hero;
        private float _cd;

        private void OnEnable() => ArcProjectile.OnHeroHitEnemy += OnHitEvent;
        private void OnDisable() => ArcProjectile.OnHeroHitEnemy -= OnHitEvent;

        public void Init(Transform hero) => _hero = hero;

        public void OnHeroHit()
        {
            if (_hero == null) return;
            if (_cd > 0f) return;

            var p = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
            p.Launch(firePoint.position, _hero.position, damagePerShot);

            _cd = shotCooldown;
        }

        private void Update()
        {
            if (_cd > 0f) _cd -= Time.deltaTime;
        }

        private void OnHitEvent(EnemyController enemy, int dmg)
        {
            if (enemy == null || enemy.gameObject != gameObject) return;
            OnHeroHit();
        }
    }
}