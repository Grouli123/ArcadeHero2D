using System.Collections.Generic;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyLaneController : MonoBehaviour
    {
        [SerializeField] private float minSpacing = 0.8f;
        private readonly List<EnemyController> _enemies = new();

        public float GetSpawnX(float baseX)
        {
            if (_enemies.Count == 0) return baseX;
            float lastX = _enemies[_enemies.Count - 1].transform.position.x;
            return Mathf.Max(baseX, lastX + minSpacing);
        }

        public void Register(EnemyController e)
        {
            if (_enemies.Contains(e)) return;
            _enemies.Add(e);
            e.Health.OnDied += () => Unregister(e);
        }

        public void Unregister(EnemyController e)
        {
            _enemies.Remove(e);
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