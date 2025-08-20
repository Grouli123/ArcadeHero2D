using UnityEngine;

namespace ArcadeHero2D.Data.Waves
{
    [System.Serializable]
    public class EnemyEntry
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnInterval = 1f;
    }

    [CreateAssetMenu(fileName = "WaveConfig", menuName = "ArcadeHero2D/Wave Config", order = 0)]
    public class WaveConfig : ScriptableObject
    {
        public EnemyEntry[] enemies;
    }
}