using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Projectiles
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class LinearProjectile : ProjectileBase
    {
        [SerializeField] private float speed = 6f;
        [SerializeField] private LayerMask targetMask;

        private Vector2 _dir;

        public override void Launch(Vector2 origin, Vector2 target, int damage)
        {
            transform.position = origin;
            _damage = damage;
            _dir = (target - origin).normalized;
        }

        private void Update()
        {
            transform.Translate(_dir * speed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & targetMask.value) == 0) return;
            if (other.TryGetComponent<IDamageable>(out var d))
                d.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}