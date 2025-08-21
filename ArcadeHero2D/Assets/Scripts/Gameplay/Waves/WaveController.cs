using System.Collections;
using ArcadeHero2D.Core.Flow;
using ArcadeHero2D.Data.Waves;
using ArcadeHero2D.Gameplay.Enemy;
using ArcadeHero2D.Minigame;
using ArcadeHero2D.UI.HUD;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Waves
{
    public sealed class WaveController : MonoBehaviour
    {
        [SerializeField] private WaveConfig wave;
        [SerializeField] private EnemyFactory factory;
        [SerializeField] private Transform hero;
        [SerializeField] private JourneyProgressBar journeyBar;
        [SerializeField] private CoinMiniGameController miniGame;
        [SerializeField] private UI.WaveResultPanel resultPanel;

        [SerializeField] private float journeyDistance = 6f;

        private int _alive;
        private float _startX;
        private float _targetX;

        private void Start()
        {
            _startX = hero.position.x;
            _targetX = _startX + journeyDistance;
            journeyBar.Bind(hero, _startX, _targetX);
            StartCoroutine(Flow());
        }

        IEnumerator Flow()
        {
            GameFlowController.Instance.SetPhase(GamePhase.Journey);
            while (hero.position.x < _targetX) yield return null;

            GameFlowController.Instance.SetPhase(GamePhase.Battle);
            yield return StartCoroutine(RunWave());

            GameFlowController.Instance.SetPhase(GamePhase.CoinMiniGame);
            bool finished = false;
            miniGame.OnFinished = () => finished = true;
            miniGame.StartMiniGame();
            while (!finished) yield return null;

            GameFlowController.Instance.SetPhase(GamePhase.Upgrade);
            resultPanel.Show();
        }

        IEnumerator RunWave()
        {
            foreach (var entry in wave.enemies)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    float spawnX = hero.position.x + 6f + i * 0.5f;
                    float spawnY = hero.position.y;
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
        }
    }
}
