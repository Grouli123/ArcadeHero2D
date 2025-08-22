using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class BounceTrigger : MonoBehaviour
    {
        [SerializeField] private float impulse = 6f;
        private void Awake() => GetComponent<Collider2D>().isTrigger = true;

        private void OnTriggerEnter2D(Collider2D other) => Handle(other);
        private void OnTriggerStay2D  (Collider2D other) => Handle(other);

        private void Handle(Collider2D other)
        {
            if (!other.TryGetComponent<Coin>(out var coin)) return;
            coin.Body.AddForce(Vector2.up * impulse, ForceMode2D.Impulse);
        }
    }
}