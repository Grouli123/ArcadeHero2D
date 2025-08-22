using System.Linq;
using ArcadeHero2D.Data.Upgrades;
using ArcadeHero2D.Domain.Contracts;

namespace ArcadeHero2D.Services.Upgrades
{
    public sealed class UpgradeService : IUpgradeService
    {
        private readonly ICurrencyService _currency;
        private readonly IStatsService _stats;
        private readonly UpgradeDefinition[] _pool;

        public UpgradeService(ICurrencyService currency, IStatsService stats, UpgradeDefinition[] pool)
        {
            _currency = currency; _stats = stats; _pool = pool;
        }

        public UpgradeDefinition[] Roll3()
        {
            return _pool.OrderBy(_ => UnityEngine.Random.value).Take(3).ToArray();
        }

        public bool TryApply(UpgradeDefinition def)
        {
            if (!_currency.TrySpend(def.price)) return false;

            switch (def.statType)
            {
                case StatType.Attack:      _stats.AddAttack(def.intValue); break;
                case StatType.AttackSpeed: _stats.AddAttackSpeed(def.floatValue); break;
                case StatType.MaxHP:       _stats.AddMaxHP(def.intValue, true); break;
            }
            return true;
        }
    }
}