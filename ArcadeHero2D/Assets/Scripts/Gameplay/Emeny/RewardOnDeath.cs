using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Base;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    [RequireComponent(typeof(EnemyController))]
    public sealed class RewardOnDeath : MonoBehaviour
    {
        [SerializeField] int reward = 1;
        ICurrencyService _currency;
        EnemyController _enemy;

        void Awake()
        {
            _currency = ServiceLocator.Get<ICurrencyService>();
            _enemy = GetComponent<EnemyController>();
            _enemy.Health.OnDied += OnDead;
        }

        void OnDead() => _currency.Add(reward);
    }
}