using System.Collections;
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
        private bool _bound;

        private void Awake()
        {
            _cup = ServiceLocator.Get<ICupBankService>();
            _enemy = GetComponent<EnemyController>();
        }

        private void OnEnable() => StartCoroutine(BindWhenReady());
        private void OnDisable()
        {
            if (_bound && _enemy != null && _enemy.Health != null)
                _enemy.Health.OnDied -= OnDead;
            _bound = false;
        }

        private IEnumerator BindWhenReady()
        {
            // ждём пока UnitBase.Awake создаст Health
            int safety = 10;
            while (_enemy.Health == null && safety-- > 0)
                yield return null;

            if (_enemy.Health != null && !_bound)
            {
                _enemy.Health.OnDied += OnDead;
                _bound = true;
            }
            else if (_enemy.Health == null)
            {
                Debug.LogError($"{name}: RewardOnDeath — Health not ready, can't bind");
            }
        }

        private void OnDead()
        {
            if (reward > 0) _cup.AddBuffered(reward);
        }
    }
}