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
        [SerializeField] private MonoBehaviour[] disableOnDeath;   
        [SerializeField] private Collider2D[]   disableColliders;  
        [SerializeField] private Rigidbody2D     rb;               

        [Header("Despawn")]
        [SerializeField] private bool destroyOnDeath = true;       
        [SerializeField] private float extraLifetime = 0.15f;      

        private UnitAnimationController _anim;
        private IHealth _health;
        private bool _handled;

        private void Awake()
        {
            _anim = GetComponent<UnitAnimationController>();

            var unit = GetComponentInParent<UnitBase>();
            _health  = unit ? unit.Health : null;

            if (rb == null) rb = GetComponentInParent<Rigidbody2D>();
            if (disableColliders == null || disableColliders.Length == 0)
                disableColliders = GetComponentsInChildren<Collider2D>(true);
        }

        private void OnEnable()
        {
            if (_health != null) _health.OnDied += OnDied;
        }

        private void OnDisable()
        {
            if (_health != null) _health.OnDied -= OnDied;
        }

        private void OnDied()
        {
            if (_handled) return;
            _handled = true;
            StartCoroutine(DoDeath());
        }

        private IEnumerator DoDeath()
        {
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

            float dur = _anim != null ? _anim.PlayDeath() : 0.6f;

            yield return new WaitForSeconds(dur + extraLifetime);

            if (destroyOnDeath && this != null && gameObject != null)
                Destroy(gameObject);
        }
    }
}
