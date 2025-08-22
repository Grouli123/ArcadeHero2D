using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class Coin : MonoBehaviour
    {
        public int Value { get; private set; } = 1;
        public Rigidbody2D Body { get; private set; }
        Collider2D _col;

        void Awake()
        {
            Body = GetComponent<Rigidbody2D>();
            _col  = GetComponent<Collider2D>();

            Body.bodyType = RigidbodyType2D.Dynamic;
            Body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Body.interpolation = RigidbodyInterpolation2D.Interpolate;

            _col.isTrigger = false; // монета — НЕ триггер
        }

        public void SetValue(int v) => Value = Mathf.Max(1, v);
        public void Multiply(int k) => Value = Mathf.Max(1, Value * Mathf.Max(1, k));
    }
}