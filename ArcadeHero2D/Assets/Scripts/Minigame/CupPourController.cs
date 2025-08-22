using System;
using System.Collections;
using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    public sealed class CupPourController : MonoBehaviour
    {
        [SerializeField] private Coin coinPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float coinsPerSecond = 4f; 
        [SerializeField] private float initialImpulse = 0.35f;

        public bool Pouring { get; private set; }
        public Action OnAllSpawned;

        private int _left;
        private Coroutine _routine;

        public void ConfigureRate(float cps) => coinsPerSecond = Mathf.Max(0.1f, cps);

        public void Begin(int count)
        {
            StopNow();
            _left = Mathf.Max(0, count);
            _routine = StartCoroutine(PourRoutine());
        }

        public void StopNow()
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = null;
            Pouring = false;
        }

        private IEnumerator PourRoutine()
        {
            if (_left <= 0) { OnAllSpawned?.Invoke(); yield break; }

            Pouring = true;
            float interval = 1f / coinsPerSecond;

            while (_left > 0)
            {
                SpawnOne();
                _left--;
                yield return new WaitForSeconds(interval);
            }

            Pouring = false;
            OnAllSpawned?.Invoke();
        }

        private void SpawnOne()
        {
            var coin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity);
            if (coin.Body != null)
            {
                coin.Body.AddForce(Vector2.right * UnityEngine.Random.Range(-initialImpulse, initialImpulse), ForceMode2D.Impulse);
            }
        }
    }
}