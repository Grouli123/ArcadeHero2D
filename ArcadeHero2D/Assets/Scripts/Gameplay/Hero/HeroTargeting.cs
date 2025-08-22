using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Hero
{
    public sealed class HeroTargeting : MonoBehaviour, ITargetProvider
    {
        [SerializeField] private float radius = 3f;
        [SerializeField] private LayerMask enemyMask;

        public Transform CurrentTarget { get; private set; }
        public bool HasTarget => CurrentTarget != null;

        private void Update()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
            float best = float.MaxValue;
            CurrentTarget = null;

            foreach (var h in hits)
            {
                float dx = h.transform.position.x - transform.position.x;
                if (dx < -0.1f) continue; 
                float d = Mathf.Abs(dx);
                if (d < best) { best = d; CurrentTarget = h.transform; }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}