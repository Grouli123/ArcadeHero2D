using System.Collections;
using ArcadeHero2D.Data.Waves;
using ArcadeHero2D.Gameplay.Enemy;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Waves
{
    public sealed class WaveController : MonoBehaviour
    {
        [SerializeField] WaveConfig wave;
        [SerializeField] EnemyFactory factory;
        [SerializeField] Transform hero;
        [SerializeField] UI.WaveResultPanel resultPanel;

        int _alive;

        void Start() => StartCoroutine(RunWave());

        IEnumerator RunWave()
        {
            foreach (var entry in wave.enemies)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    float spawnX = hero.position.x + 6f + i * 0.5f;   // сдвиг вперёд от героя
                    float spawnY = hero.position.y;                    // тот же Y, что у героя
                    var pos = new Vector3(spawnX, spawnY, 0f);

                    var e = factory.Spawn(entry.enemyPrefab.GetComponent<EnemyController>(), pos);
                    e.Init(hero);
                    _alive++;
                    e.Health.OnDied += () => { _alive--; };
                    yield return new WaitForSeconds(entry.spawnInterval);
                }
                yield return null;
            }

            while (_alive > 0) yield return null;

            resultPanel.Show();
        }
    }
}