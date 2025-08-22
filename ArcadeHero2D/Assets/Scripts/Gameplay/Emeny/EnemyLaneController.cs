using System.Collections.Generic;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyLaneController : MonoBehaviour
    {
        [SerializeField] private float minSpacing = 0.8f;
        [SerializeField] private float spawnOffset = 0.9f;  // БЫЛО 2.5f

        private readonly List<EnemyController> _enemies = new();
        private float _nextSlotX;

        public void ResetLane(float startSlotX)
        {
            _nextSlotX = startSlotX;
            _enemies.Clear();
        }

        public float ReserveSlotX()
        {
            float slot = _nextSlotX;
            _nextSlotX += minSpacing;
            return slot;
        }

        public float GetSpawnXForSlot(float slotX) => slotX + spawnOffset;

        public void Register(EnemyController e)
        {
            if (_enemies.Contains(e)) return;
            _enemies.Add(e);
            e.Health.OnDied += () => _enemies.Remove(e);
        }

        public float GetAllowedStepX(EnemyController e, float desiredX, float customMinSpacing)
        {
            int i = _enemies.IndexOf(e);
            if (i > 0)
            {
                var front = _enemies[i - 1];
                float limit = front.transform.position.x + Mathf.Max(minSpacing, customMinSpacing);
                if (desiredX < limit) desiredX = limit;
            }
            return desiredX;
        }
    }
}