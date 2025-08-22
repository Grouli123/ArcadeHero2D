using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private Transform spawnRoot;

        public EnemyController Spawn(EnemyController prefab, Vector3 pos)
        {
            if (prefab == null) { Debug.LogError("[Factory] Prefab is null"); return null; }
            if (spawnRoot == null) { Debug.LogError("[Factory] spawnRoot not set"); return null; }

            var e = Instantiate(prefab, pos, Quaternion.identity, spawnRoot);
            e.gameObject.name = prefab.name + "_Clone";
            e.gameObject.SetActive(true);
            Debug.LogWarning($"[Factory] Spawned {e.name} @ {pos}"); 
            return e;
        }
    }
}