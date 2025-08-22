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
using ArcadeHero2D.Domain.Base; 

namespace ArcadeHero2D.Gameplay.Waves
{
    public sealed class WaveSequenceController : MonoBehaviour
    {
        public event System.Action OnSequenceCompleted;
        
        [Header("Refs")]
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

        [SerializeField] private UnitBase heroUnit;               
        private IHealth _heroHealth;
        private bool _heroDead;

        private void Awake()
        {
            _heroAttack = hero ? hero.GetComponentInChildren<HeroAttackController>() : null;
            _heroMover  = hero ? hero.GetComponent<HeroMover>() : null;

            if (turnController == null) turnController = gameObject.AddComponent<BattleTurnController>();
            if (_heroAttack != null) turnController.Bind(_heroAttack, hero);

            _cup = ServiceLocator.Get<ICupBankService>();

            if (heroUnit == null && hero != null) heroUnit = hero.GetComponent<UnitBase>();
            if (heroUnit != null && heroUnit.Health != null)
            {
                _heroHealth = heroUnit.Health;
                _heroHealth.OnDied += OnHeroDied;
            }
        }

        private void OnDestroy()
        {
            if (_heroHealth != null) _heroHealth.OnDied -= OnHeroDied;
        }

        private void Start()
        {
            if (!CheckRefs()) { enabled = false; return; }
            StartCoroutine(RunSequence());
        }

        private bool CheckRefs()
        {
            if (journeyBar == null || miniGame == null || resultPanel == null || factory == null || lane == null)
            { Debug.LogError("[WaveSeq] Missing refs"); return false; }
            if (waves == null || waves.Length == 0)
            { Debug.LogError("[WaveSeq] waves[] is empty"); return false; }
            return true;
        }

        private void OnHeroDied()
        {
            _heroDead = true;
            if (_heroMover != null) _heroMover.Stop();
            if (_heroAttack != null) _heroAttack.AttackEnabled = false;
        }

        private IEnumerator RunSequence()
        {
            for (int w = 0; w < waves.Length; w++)
            {
                if (_heroDead) yield break;

                float startX = SafeHeroX();               
                float targetX = startX + journeyDistance;
                if (hero != null) journeyBar.Bind(hero, startX, targetX);

                if (_heroAttack != null) _heroAttack.AttackEnabled = false;
                if (_heroMover  != null) _heroMover.Resume();

                GameFlowController.Instance.SetPhase(GamePhase.Journey);

                float t = 0f, timeout = 12f;
                while (!_heroDead && hero != null && SafeHeroX() < targetX && t < timeout)
                { t += Time.deltaTime; yield return null; }

                if (_heroMover != null) _heroMover.Stop();

                if (_heroDead || hero == null) yield break;

                if (SafeHeroX() < targetX)
                    SnapHeroX(targetX);

                List<EnemyController> prepared = null;
                yield return StartCoroutine(PrepareWaveStanding(waves[w], list => prepared = list));
                if (_heroDead) yield break;

                if (w > 0)
                {
                    yield return StartCoroutine(PlayMiniGameAndCards());
                    if (_heroDead) yield break;
                }

                if (_heroAttack != null) _heroAttack.AttackEnabled = true;
                if (turnController != null)
                    yield return StartCoroutine(turnController.RunBattle(prepared));
            }
            OnSequenceCompleted?.Invoke();   
        }

        private IEnumerator PrepareWaveStanding(WaveConfig wave, System.Action<List<EnemyController>> onReady)
        {
            var spawned = new List<EnemyController>();

            float heroX = SafeHeroX();
            var cam = Camera.main;
            float halfW = cam.orthographicSize * cam.aspect;
            float slotMarginFromHero = 1.6f;
            float slotRightPadding   = 0.8f;
            float startSlotX = heroX + Mathf.Min(Mathf.Max(slotMarginFromHero, halfW - slotRightPadding), 2.2f);

            lane.ResetLane(startSlotX);

            foreach (var entry in wave.enemies)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    if (_heroDead) { CleanupSpawned(spawned); yield break; }

                    var prefab = entry.enemyPrefab ? entry.enemyPrefab.GetComponent<EnemyController>() : null;
                    if (prefab == null) continue;

                    float slotX  = lane.ReserveSlotX();
                    float spawnX = lane.GetSpawnXForSlot(slotX);
                    float spawnY = hero ? hero.position.y : 0f;

                    var e = factory.Spawn(prefab, new Vector3(spawnX, spawnY, 0f));
                    if (e == null) continue;

                    e.AssignSlot(slotX);
                    if (hero != null) e.Init(hero);

                    var melee = e.GetComponent<EnemyMeleeResponder>();
                    if (melee != null) melee.lane = lane;

                    lane.Register(e);
                    spawned.Add(e);

                    yield return new WaitForSeconds(entry.spawnInterval);
                }
            }

            bool allReady = false;
            while (!allReady)
            {
                if (_heroDead) { CleanupSpawned(spawned); yield break; }

                allReady = true;
                for (int i = 0; i < spawned.Count; i++)
                {
                    var e = spawned[i];
                    if (e == null) continue;
                    if (!e.IsReady) { allReady = false; break; }
                }
                yield return null;
            }

            onReady?.Invoke(spawned);
        }

        private IEnumerator PlayMiniGameAndCards()
        {
            GameFlowController.Instance.SetPhase(GamePhase.CoinMiniGame);
            bool finished = false;
            miniGame.OnFinished = () => finished = true;
            miniGame.StartMiniGame();
            while (!finished && !_heroDead) yield return null;
            if (_heroDead) yield break;

            GameFlowController.Instance.SetPhase(GamePhase.Upgrade);
            bool chosen = false;
            resultPanel.OnUpgradeChosen = () => chosen = true;
            resultPanel.Show();
            while (!chosen && !_heroDead) yield return null;
        }

        private float SafeHeroX()
        {
            if (_heroDead || hero == null) return 0f;
            try { return hero.position.x; }
            catch { return 0f; }
        }

        private void SnapHeroX(float x)
        {
            if (_heroDead || hero == null) return;
            try
            {
                var p = hero.position;
                p.x = x;
                hero.position = p;
            }
            catch { }
        }

        private void CleanupSpawned(List<EnemyController> list)
        {
            
        }
    }
}
