using System;
using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CoinCollector : MonoBehaviour
    {
        public event Action<int> OnCoinCollected;
        public int TotalCollected { get; private set; }
        public int CollectedCount { get; private set; }

        private void Awake() => GetComponent<Collider2D>().isTrigger = true; 

        private void OnTriggerEnter2D(Collider2D other) => Handle(other);
        private void OnTriggerStay2D  (Collider2D other) => Handle(other);

        private void Handle(Collider2D other)
        {
            if (!other.TryGetComponent<Coin>(out var coin)) return;
            TotalCollected += coin.Value;
            CollectedCount += 1;
            OnCoinCollected?.Invoke(coin.Value);
            Destroy(other.gameObject);
        }

        public void ResetSum()
        {
            TotalCollected = 0;
            CollectedCount = 0;
        }
    }
}