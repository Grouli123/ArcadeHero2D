using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(CoinMultiplierZone))]
    public sealed class SecretMultiplierZone : MonoBehaviour
    {
        [SerializeField] private int[] possible = new int[] { 2, 3, 4 };
        [SerializeField] private GameObject hiddenVisual; 

        private CoinMultiplierZone _zone;
        private bool _revealed;

        private void Awake()
        {
            _zone = GetComponent<CoinMultiplierZone>();
            if (hiddenVisual) hiddenVisual.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var coin = other.GetComponentInParent<Coin>();
            if (coin == null) return;

            if (!_revealed)
            {
                int f = possible != null && possible.Length > 0
                    ? possible[Random.Range(0, possible.Length)]
                    : 2;

                _zone.SetFactor(f);
                if (hiddenVisual) hiddenVisual.SetActive(false);
                _revealed = true;
            }
        }
    }
}