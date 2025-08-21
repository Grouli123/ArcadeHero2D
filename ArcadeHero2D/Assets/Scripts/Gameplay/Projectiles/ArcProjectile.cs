// ArcProjectile.cs
using System.Collections;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Projectiles
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class ArcProjectile : ProjectileBase
    {
        [SerializeField] float flightTime = 0.4f;
        [SerializeField] AnimationCurve heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
        [SerializeField] LayerMask enemyMask;

        Vector2 _start, _end;

        public override void Launch(Vector2 origin, Vector2 target, int damage)
        {
            _damage = damage;
            _start = origin; _end = target;
            StartCoroutine(Fly());
        }

        IEnumerator Fly()
        {
            float t = 0f;
            while (t < flightTime)
            {
                t += Time.deltaTime;
                float n = t / flightTime;
                var pos = Vector2.Lerp(_start, _end, n);
                pos.y += heightCurve.Evaluate(n);
                transform.position = pos;
                yield return null;
            }
            Destroy(gameObject);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & enemyMask.value) == 0) return;
            if (other.TryGetComponent<IDamageable>(out var d)) d.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}