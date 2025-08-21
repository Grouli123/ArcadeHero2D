using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class Coin : MonoBehaviour
    {
        public int Value { get; private set; } = 1;
        public Rigidbody2D Body { get; private set; }

        private void Awake() => Body = GetComponent<Rigidbody2D>();
        public void SetValue(int v) => Value = Mathf.Max(1, v);
        public void Multiply(int k) => Value = Mathf.Max(1, Value * Mathf.Max(1, k));
    }
}