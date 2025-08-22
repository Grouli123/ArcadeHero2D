using System;
using System.Collections;
using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    public sealed class CupPourController : MonoBehaviour
    {
        [SerializeField] Coin coinPrefab;
        [SerializeField] Transform spawnPoint;
        [SerializeField] float coinsPerSecond = 4f; // медленнее по умолчанию
        [SerializeField] float initialImpulse = 0.35f;

        public bool Pouring { get; private set; }
        public Action OnAllSpawned;

        int _left;
        Coroutine _routine;

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

        IEnumerator PourRoutine()
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

        void SpawnOne()
        {
            var coin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity);
            if (coin.Body != null)
            {
                coin.Body.AddForce(Vector2.right * UnityEngine.Random.Range(-initialImpulse, initialImpulse), ForceMode2D.Impulse);
            }
        }
    }
}