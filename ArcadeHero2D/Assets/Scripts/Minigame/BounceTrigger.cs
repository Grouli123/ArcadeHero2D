using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class BounceTrigger : MonoBehaviour
    {
        [SerializeField] float impulse = 6f;
        void Awake() => GetComponent<Collider2D>().isTrigger = true;

        void OnTriggerEnter2D(Collider2D other) => Handle(other);
        void OnTriggerStay2D  (Collider2D other) => Handle(other);

        void Handle(Collider2D other)
        {
            if (!other.TryGetComponent<Coin>(out var coin)) return;
            coin.Body.AddForce(Vector2.up * impulse, ForceMode2D.Impulse);
        }
    }
}