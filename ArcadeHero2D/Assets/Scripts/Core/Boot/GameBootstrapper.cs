using ArcadeHero2D.Core.Flow;
using ArcadeHero2D.Data.Units;
using ArcadeHero2D.Domain.Contracts;
using ArcadeHero2D.Services.Currency;
using ArcadeHero2D.Services.CupBank;
using ArcadeHero2D.Services.Stats;
using ArcadeHero2D.Services.Upgrades;
using UnityEngine;

namespace ArcadeHero2D.Core.Boot
{
    [DefaultExecutionOrder(-1000)]
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Configs (SO)")]
        [SerializeField] private UnitStats heroStats;
        [SerializeField] private ArcadeHero2D.Data.Upgrades.UpgradeDefinition[] upgradePool;

        private void Awake()
        {
            ServiceLocator.Clear();

            var currency = new CurrencyService();
            var cupBank  = new CupBankService();
            var stats    = new StatsService(heroStats);
            var upgrades = new UpgradeService(currency, stats, upgradePool);

            ServiceLocator.Register<ICurrencyService>(currency);
            ServiceLocator.Register<ICupBankService>(cupBank);
            ServiceLocator.Register<IStatsService>(stats);
            ServiceLocator.Register<IUpgradeService>(upgrades);

            if (GameFlowController.Instance == null)
            {
                var go = new GameObject("GameFlow");
                go.AddComponent<GameFlowController>();
            }
        }
    }
}