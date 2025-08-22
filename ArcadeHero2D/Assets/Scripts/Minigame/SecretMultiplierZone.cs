using UnityEngine;
// using TMPro;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(CoinMultiplierZone))]
    public sealed class SecretMultiplierZone : MonoBehaviour
    {
        [SerializeField] int[] possible = new int[] { 2, 3, 4 };
        [SerializeField] GameObject hiddenVisual; // закрытая плитка/вопросик
        // [SerializeField] TMP_Text revealLabel;  // опциональная подпись "x?"

        CoinMultiplierZone _zone;
        bool _revealed;

        void Awake()
        {
            _zone = GetComponent<CoinMultiplierZone>();
            if (hiddenVisual) hiddenVisual.SetActive(true);
            // if (revealLabel) revealLabel.text = "x?";
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var coin = other.GetComponentInParent<Coin>();
            if (coin == null) return;

            if (!_revealed)
            {
                int f = possible != null && possible.Length > 0
                    ? possible[Random.Range(0, possible.Length)]
                    : 2;

                _zone.SetFactor(f/*, revealLabel*/);
                if (hiddenVisual) hiddenVisual.SetActive(false);
                // if (revealLabel) revealLabel.text = $"x{f}";
                _revealed = true;

                // ВАЖНО: после раскрытия позволяем зоне обработать это же столкновение —
                // умножение произойдёт в CoinMultiplierZone.OnTriggerEnter2D на следующем кадре
            }
            // Ничего больше не делаем: CoinMultiplierZone отвечает за само умножение.
        }
    }
}