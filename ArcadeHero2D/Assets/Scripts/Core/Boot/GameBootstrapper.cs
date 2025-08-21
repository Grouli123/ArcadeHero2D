using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Data.Units;
using ArcadeHero2D.Domain.Contracts;
using ArcadeHero2D.Services.Currency;
using ArcadeHero2D.Services.Stats;
using ArcadeHero2D.Services.Upgrades;
using UnityEngine;

namespace ArcadeHero2D.Core.Boot
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Configs (SO)")]
        [SerializeField] UnitStats heroStats;
        [SerializeField] ArcadeHero2D.Data.Upgrades.UpgradeDefinition[] upgradePool;

        void Awake()
        {
            ServiceLocator.Clear();
            var currency = new CurrencyService();
            var stats = new StatsService(heroStats);
            var upgrades = new UpgradeService(currency, stats, upgradePool);

            ServiceLocator.Register<ICurrencyService>(currency);
            ServiceLocator.Register<IStatsService>(stats);
            ServiceLocator.Register<IUpgradeService>(upgrades);
        }
    }
}