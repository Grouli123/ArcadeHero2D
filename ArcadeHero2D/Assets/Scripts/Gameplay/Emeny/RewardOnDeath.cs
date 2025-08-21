using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Base;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    [RequireComponent(typeof(EnemyController))]
    public sealed class RewardOnDeath : MonoBehaviour
    {
        [SerializeField] private int reward = 1;
        private ICupBankService _cup;
        private EnemyController _enemy;

        private void Awake()
        {
            _cup = ServiceLocator.Get<ICupBankService>();
            _enemy = GetComponent<EnemyController>();
            _enemy.Health.OnDied += OnDead;
        }

        private void OnDead() => _cup.AddBuffered(reward);
    }
}