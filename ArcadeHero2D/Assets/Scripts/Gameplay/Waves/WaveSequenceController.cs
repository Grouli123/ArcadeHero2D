using System.Collections;
using System.Collections.Generic;
using ArcadeHero2D.Core.Flow;
using ArcadeHero2D.Data.Waves;
using ArcadeHero2D.Gameplay.Battle;
using ArcadeHero2D.Gameplay.Enemy;
using ArcadeHero2D.Gameplay.Hero;
using ArcadeHero2D.Minigame;
using ArcadeHero2D.UI.HUD;
using UnityEngine;
using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Contracts;

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
        [SerializeField] private BattleTurnController turnController;

        private HeroAttackController _heroAttack;
        private HeroMover _heroMover;
        private ICupBankService _cup;

        void Awake()
        {
            _heroAttack = hero != null ? hero.GetComponentInChildren<HeroAttackController>() : null;
            _heroMover  = hero != null ? hero.GetComponent<HeroMover>() : null;

            if (turnController == null) turnController = gameObject.AddComponent<BattleTurnController>();
            if (_heroAttack != null) turnController.Bind(_heroAttack, hero);

            _cup = ServiceLocator.Get<ICupBankService>();
        }

        void Start()
        {
            if (hero == null || journeyBar == null || miniGame == null || resultPanel == null || factory == null || lane == null)
            { Debug.LogError("[WaveSeq] Missing refs"); enabled = false; return; }
            if (waves == null || waves.Length == 0)
            { Debug.LogError("[WaveSeq] waves[] is empty"); enabled = false; return; }

            StartCoroutine(RunSequence());
        }

        IEnumerator RunSequence()
        {
            for (int w = 0; w < waves.Length; w++)
            {
                // 1) Путь к следующей волне
                float startX = hero.position.x;
                float targetX = startX + journeyDistance;
                journeyBar.Bind(hero, startX, targetX);

                if (_heroAttack != null) _heroAttack.AttackEnabled = false; // герой не стреляет
                if (_heroMover  != null) _heroMover.Resume();

                GameFlowController.Instance.SetPhase(GamePhase.Journey);
                float t = 0f, timeout = 12f;
                while (hero.position.x < targetX && t < timeout) { t += Time.deltaTime; yield return null; }
                if (_heroMover != null) _heroMover.Stop();
                if (hero.position.x < targetX) { var p = hero.position; p.x = targetX; hero.position = p; }

                // 2) Расставляем текущую волну: враги подъезжают в слоты и стоят (бой не начинается)
                List<EnemyController> prepared = null;
                yield return StartCoroutine(PrepareWaveStanding(waves[w], readyList => prepared = readyList));

                // 3) Для второй и последующих волн — СНАЧАЛА мини-игра предыдущей волны и карточки
                if (w > 0)
                {
                    yield return StartCoroutine(PlayMiniGameAndCards());
                }

                // 4) Запускаем пошаговый бой для ТЕКУЩЕЙ волны
                if (_heroAttack != null) _heroAttack.AttackEnabled = true;
                if (turnController != null) yield return StartCoroutine(turnController.RunBattle(prepared));
            }

            // Примечание: по твоему циклу мини-игра запускается "на подходе к следующей волне".
            // После последней волны дополнительной мини-игры нет. Если нужно — скажи, добавим финальную выдачу монет.
        }

        IEnumerator PrepareWaveStanding(WaveConfig wave, System.Action<List<EnemyController>> onReady)
        {
            var spawned = new List<EnemyController>();

            var cam = Camera.main;
            float halfW = cam.orthographicSize * cam.aspect;
            float slotMarginFromHero = 1.6f;
            float slotRightPadding   = 0.8f;
            float startSlotX = hero.position.x + Mathf.Min(Mathf.Max(slotMarginFromHero, halfW - slotRightPadding), 2.2f);

            lane.ResetLane(startSlotX);

            foreach (var entry in wave.enemies)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    var prefab = entry.enemyPrefab != null ? entry.enemyPrefab.GetComponent<EnemyController>() : null;
                    if (prefab == null) continue;

                    float slotX = lane.ReserveSlotX();
                    float spawnX = lane.GetSpawnXForSlot(slotX);
                    float spawnY = hero.position.y;
                    var pos = new Vector3(spawnX, spawnY, 0f);

                    var e = factory.Spawn(prefab, pos);
                    if (e == null) continue;

                    e.AssignSlot(slotX);
                    e.Init(hero);

                    var melee = e.GetComponent<EnemyMeleeResponder>();
                    if (melee != null) melee.lane = lane;

                    lane.Register(e);
                    spawned.Add(e);

                    yield return new WaitForSeconds(entry.spawnInterval);
                }
            }

            // ждём, пока все встанут в свои слоты (InSlot)
            bool allReady = false;
            while (!allReady)
            {
                allReady = true;
                for (int i = 0; i < spawned.Count; i++)
                {
                    var e = spawned[i];
                    if (e == null) continue;
                    if (!e.IsReady) { allReady = false; break; }
                }
                yield return null;
            }

            // Герой всё ещё с выключенной атакой; враги просто стоят, ждут начала боя
            onReady?.Invoke(spawned);
        }

        IEnumerator PlayMiniGameAndCards()
        {
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
}
