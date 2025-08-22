using UnityEngine;
// Если используешь TMP для подписи, раскомментируй следующую строку и поля label
// using TMPro;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CoinMultiplierZone : MonoBehaviour
    {
        [Header("Multiplier")]
        [SerializeField] int factor = 2;

        [Header("Spawning")]
        [SerializeField] Coin coinPrefab;                  // тот же префаб, что сыплет CupPourController
        [SerializeField] float scatterImpulse = 0.8f;      // сила «разлёта» новых монет
        [SerializeField] float upwardBias = 0.25f;         // небольшой подъём
        [SerializeField] float reenterGuard = 0.08f;       // игнорировать монеты только что заспавненные

        // [SerializeField] TMP_Text label;               // опционально: подпись "x2/x3/x4"
        int _zoneId;

        void Awake()
        {
            _zoneId = GetInstanceID();
            var c = GetComponent<Collider2D>(); c.isTrigger = true;
            // if (label) label.text = $"x{factor}";
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var coin = other.GetComponentInParent<Coin>();
            if (coin == null) return;

            // Защита: монета только что создана внутри триггера — не умножаем мгновенно
            if (coin.IsFresh(reenterGuard)) return;

            // Защита от мгновенной «пере-активации» этой же зоны
            if (coin.LastZoneId == _zoneId && Time.time - coin.BornTime < 0.25f) return;

            MultiplyAt(coin.transform.position);
            Destroy(coin.gameObject);
        }

        void MultiplyAt(Vector3 pos)
        {
            for (int i = 0; i < Mathf.Max(1, factor); i++)
            {
                var c = Instantiate(coinPrefab, pos, Quaternion.identity);
                c.LastZoneId = _zoneId;

                var rb = c.Body;
                if (rb != null)
                {
                    // случайный разлёт с небольшим уклоном вверх
                    Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(0.15f, 1f)).normalized;
                    rb.AddForce((dir + Vector2.up * upwardBias) * scatterImpulse, ForceMode2D.Impulse);
                    rb.AddTorque(Random.Range(-30f, 30f));
                }
            }
        }

        // На всякий — публичный сеттер, если хочешь менять множитель из инспектора/кода
        public void SetFactor(int f/*, TMP_Text lbl = null*/)
        {
            factor = Mathf.Max(1, f);
            // if (lbl != null) label = lbl;
            // if (label) label.text = $"x{factor}";
        }
    }
}
