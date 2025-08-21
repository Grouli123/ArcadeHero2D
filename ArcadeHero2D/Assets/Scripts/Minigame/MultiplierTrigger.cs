using TMPro;
using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class MultiplierTrigger : MonoBehaviour
    {
        [SerializeField] private int factor = 2;
        [SerializeField] private bool mystery;
        [SerializeField] private TMP_Text label;
        private bool _revealed;

        private void Start()
        {
            if (label != null) label.text = mystery ? "???" : $"x{factor}";
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<Coin>(out var coin)) return;
            if (mystery && !_revealed)
            {
                int[] opts = new[] { 2, 3, 4 };
                factor = opts[Random.Range(0, opts.Length)];
                _revealed = true;
                if (label != null) label.text = $"x{factor}";
            }
            coin.Multiply(factor);
        }
    }
}