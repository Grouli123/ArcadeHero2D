using ArcadeHero2D.Gameplay.Projectiles;
using ArcadeHero2D.Rendering;
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
        private SlotMover _slot;
        private UnitAnimationController _anim;

        public void Init(Transform hero)
        {
            _hero = hero;
            _slot = GetComponent<SlotMover>();
            _anim = GetComponentInChildren<UnitAnimationController>();
        }

        public void OnHeroAttacked()
        {
            if (_hero == null) return;
            if (_slot != null && !_slot.InSlot) return;
            if (_cd > 0f) return;

            var p = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
            p.Launch(firePoint.position, _hero.position, damagePerShot);
            _cd = shotCooldown;

            if (_anim) _anim.RequestAttack();
        }

        private void Update()
        {
            if (_cd > 0f) _cd -= Time.deltaTime;
        }
    }
}