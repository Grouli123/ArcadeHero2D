using System;
using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CoinCollector : MonoBehaviour
    {
        public event Action<int> OnCoinCollected;
        public int TotalCollected { get; private set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<Coin>(out var coin)) return;
            TotalCollected += coin.Value;
            OnCoinCollected?.Invoke(coin.Value);
            Destroy(other.gameObject);
        }

        public void ResetSum() => TotalCollected = 0;
    }
}