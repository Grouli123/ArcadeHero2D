using System.Collections.Generic;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyLaneController : MonoBehaviour
    {
        [SerializeField] private float minSpacing = 0.8f;
        [SerializeField] private float spawnOffset = 0.9f;

        private readonly List<EnemyController> _enemies = new();
        private float _nextSlotX;

        public void ResetLane(float startSlotX)
        {
            _enemies.Clear();
            _nextSlotX = startSlotX;
        }

        public void Register(EnemyController e)
        {
            if (e != null && !_enemies.Contains(e))
                _enemies.Add(e);
        }

        public float ReserveSlotX()
        {
            float x = _nextSlotX;
            _nextSlotX += minSpacing;
            return x;
        }

        public float GetSpawnXForSlot(float slotX) => slotX + spawnOffset;

        public float GetAllowedStepX(EnemyController e, float desiredX, float customMinSpacing)
        {
            int i = _enemies.IndexOf(e);
            if (i > 0)
            {
                var front = _enemies[i - 1];
                float limit = front.transform.position.x + Mathf.Max(minSpacing, customMinSpacing) + 0.02f;
                if (desiredX < limit) desiredX = limit;
            }
            return desiredX;
        }
    }
}