using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CoinMultiplierZone : MonoBehaviour
    {
        [Header("Multiplier")]
        [SerializeField] private int factor = 2;

        [Header("Spawning")]
        [SerializeField] private Coin coinPrefab;                  
        [SerializeField] private float scatterImpulse = 0.8f;      
        [SerializeField] private float upwardBias = 0.25f;         
        [SerializeField] private float reenterGuard = 0.08f;       
      
        private int _zoneId;

        private void Awake()
        {
            _zoneId = GetInstanceID();
            var c = GetComponent<Collider2D>(); c.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var coin = other.GetComponentInParent<Coin>();
            if (coin == null) return;

            if (coin.IsFresh(reenterGuard)) return;

            if (coin.LastZoneId == _zoneId && Time.time - coin.BornTime < 0.25f) return;

            MultiplyAt(coin.transform.position);
            Destroy(coin.gameObject);
        }

        private void MultiplyAt(Vector3 pos)
        {
            for (int i = 0; i < Mathf.Max(1, factor); i++)
            {
                var c = Instantiate(coinPrefab, pos, Quaternion.identity);
                c.LastZoneId = _zoneId;

                var rb = c.Body;
                if (rb != null)
                {
                    Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(0.15f, 1f)).normalized;
                    rb.AddForce((dir + Vector2.up * upwardBias) * scatterImpulse, ForceMode2D.Impulse);
                    rb.AddTorque(Random.Range(-30f, 30f));
                }
            }
        }

        public void SetFactor(int f)
        {
            factor = Mathf.Max(1, f);
        }
    }
}
