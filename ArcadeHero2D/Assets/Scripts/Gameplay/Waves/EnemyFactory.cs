using UnityEngine;

namespace ArcadeHero2D.Gameplay.Waves
{
    public sealed class EnemyFactory : MonoBehaviour
    {
        [SerializeField] Transform spawnRoot;

        public T Spawn<T>(T prefab, Vector3 pos) where T : Component
        {
            return Instantiate(prefab, pos, Quaternion.identity, spawnRoot);
        }
    }
}