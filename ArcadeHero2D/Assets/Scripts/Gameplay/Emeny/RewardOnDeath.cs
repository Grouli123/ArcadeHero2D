using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Base;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    [DisallowMultipleComponent]
    public sealed class RewardOnDeath : MonoBehaviour
    {
        [SerializeField] private int reward = 1;

        UnitBase _unit;
        ICupBankService _cup;
        ICurrencyService _currency;

        void Awake()
        {
            _unit = GetComponent<UnitBase>();
            _cup = ServiceLocator.Get<ICupBankService>();
            _currency = ServiceLocator.Get<ICurrencyService>();

            if (_unit != null && _unit.Health != null)
                _unit.Health.OnDied += OnDied;
        }

        void OnDestroy()
        {
            if (_unit != null && _unit.Health != null)
                _unit.Health.OnDied -= OnDied;
        }

        void OnDied()
        {
            // Сначала в буфер (для мини-игры). Если сервиса нет — в кошелёк напрямую.
            if (_cup != null) _cup.AddBuffered(reward);
            else if (_currency != null) _currency.Add(reward);
        }
    }
}