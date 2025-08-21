using System.Collections;
using ArcadeHero2D.Core.Flow;
using ArcadeHero2D.Data.Waves;
using ArcadeHero2D.Gameplay.Enemy;
using ArcadeHero2D.Minigame;
using ArcadeHero2D.UI.HUD;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Waves
{
    public sealed class WaveSequenceController : MonoBehaviour
    {
        [SerializeField] private WaveConfig[] waves;
        [SerializeField] private EnemyFactory factory;
        [SerializeField] private EnemyLaneController lane;
        [SerializeField] private Transform hero;
        [SerializeField] private JourneyProgressBar journeyBar;
        [SerializeField] private CoinMiniGameController miniGame;
        [SerializeField] private UI.WaveResultPanel resultPanel;
        [SerializeField] private float journeyDistance = 6f;

        private void Awake()
        {
            if (GameFlowController.Instance == null)
            {
                var go = new GameObject("GameFlow");
                go.AddComponent<GameFlowController>();
            }
        }

        private void Start()
        {
            if (hero == null || journeyBar == null || miniGame == null || resultPanel == null)
            {
                Debug.LogError($"{name}: WaveSequenceController — hero/journeyBar/miniGame/resultPanel not set in inspector");
                enabled = false; return;
            }
            if (waves == null || waves.Length == 0)
            {
                Debug.LogError($"{name}: WaveSequenceController — waves array is empty");
                enabled = false; return;
            }
            StartCoroutine(RunSequence());
        }

        IEnumerator RunSequence()
        {
            for (int w = 0; w < waves.Length; w++)
            {
                float startX = hero.position.x;
                float targetX = startX + journeyDistance;
                journeyBar.Bind(hero, startX, targetX);

                GameFlowController.Instance.SetPhase(GamePhase.Journey);
                while (hero.position.x < targetX) yield return null;

                GameFlowController.Instance.SetPhase(GamePhase.Battle);
                yield return StartCoroutine(RunWave(waves[w]));

                GameFlowController.Instance.SetPhase(GamePhase.CoinMiniGame);
                bool finished = false;
                miniGame.OnFinished = () => finished = true;
                miniGame.StartMiniGame();
                while (!finished) yield return null;

                GameFlowController.Instance.SetPhase(GamePhase.Upgrade);
                bool chosen = false;
                resultPanel.OnUpgradeChosen = () => chosen = true;
                resultPanel.Show();
                while (!chosen) yield return null;
            }
        }

        IEnumerator RunWave(WaveConfig wave)
        {
            int alive = 0;
            foreach (var entry in wave.enemies)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    float baseX = hero.position.x + 6f + i * 0.5f;
                    float spawnX = lane != null ? lane.GetSpawnX(baseX) : baseX;
                    float spawnY = hero.position.y;
                    var pos = new Vector3(spawnX, spawnY, 0f);

                    var e = factory.Spawn(entry.enemyPrefab.GetComponent<EnemyController>(), pos);
                    var melee = e.GetComponent<EnemyMeleeResponder>();
                    if (melee != null) melee.lane = lane;
                    e.Init(hero);
                    if (lane != null) lane.Register(e);

                    alive++;
                    e.Health.OnDied += () => { alive--; };
                    yield return new WaitForSeconds(entry.spawnInterval);
                }
                yield return null;
            }
            while (alive > 0) yield return null;
        }
    }
}
