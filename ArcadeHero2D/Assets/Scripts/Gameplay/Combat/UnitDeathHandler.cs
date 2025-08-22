using System.Collections;
using ArcadeHero2D.Domain.Base;
using ArcadeHero2D.Domain.Contracts;
using ArcadeHero2D.Rendering;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UnitAnimationController))]
    public sealed class UnitDeathHandler : MonoBehaviour
    {
        [Header("Disable on death (optional)")]
        [SerializeField] private MonoBehaviour[] disableOnDeath;   // HeroMover, HeroAttackController, Enemy*Responder, SlotMover и пр.
        [SerializeField] private Collider2D[]   disableColliders;  // если не задать — найдём автоматически
        [SerializeField] private Rigidbody2D     rb;               // если есть — заморозим

        [Header("Despawn")]
        [SerializeField] private bool destroyOnDeath = true;       // ВКЛ для всех (и герой, и враги)
        [SerializeField] private float extraLifetime = 0.15f;      // небольшой запас после клипа

        UnitAnimationController _anim;
        IHealth _health;
        bool _handled;

        void Awake()
        {
            _anim = GetComponent<UnitAnimationController>();

            // найдём здоровье через UnitBase на этом/родительском GO
            var unit = GetComponentInParent<UnitBase>();
            _health  = unit ? unit.Health : null;

            if (rb == null) rb = GetComponentInParent<Rigidbody2D>();
            if (disableColliders == null || disableColliders.Length == 0)
                disableColliders = GetComponentsInChildren<Collider2D>(true);
        }

        void OnEnable()
        {
            if (_health != null) _health.OnDied += OnDied;
        }

        void OnDisable()
        {
            if (_health != null) _health.OnDied -= OnDied;
        }

        void OnDied()
        {
            if (_handled) return;
            _handled = true;
            StartCoroutine(DoDeath());
        }

        IEnumerator DoDeath()
        {
            // 1) Глушим логику, коллайдеры, физику
            if (disableOnDeath != null)
                foreach (var b in disableOnDeath) if (b) b.enabled = false;

            if (disableColliders != null)
                foreach (var c in disableColliders) if (c) c.enabled = false;

            if (rb)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.isKinematic = true;
                rb.simulated = false;
            }

            // 2) Играем смерть
            float dur = _anim != null ? _anim.PlayDeath() : 0.6f;

            // 3) Ждём конец клипа и небольшой запас, затем удаляем
            yield return new WaitForSeconds(dur + extraLifetime);

            if (destroyOnDeath && this != null && gameObject != null)
                Destroy(gameObject);
        }
    }
}
