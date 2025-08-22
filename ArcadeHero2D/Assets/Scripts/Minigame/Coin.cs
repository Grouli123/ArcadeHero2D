using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Coin : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D Body { get; private set; }

        [Header("Gameplay")]
        [SerializeField] private int value = 1;
        [SerializeField] private float spawnIgnoreTime = 0.1f;

        public int Value => value;

        public float BornTime { get; private set; }
        public int LastZoneId { get; set; } = -1;
        public bool IsFresh(float guard) => Time.time - BornTime < guard;

        private bool _markedForDestroy;

        private void Awake()
        {
            if (!Body) Body = GetComponent<Rigidbody2D>();
            var col = GetComponent<Collider2D>();
            col.isTrigger = false;
        }

        private void OnEnable()
        {
            _markedForDestroy = false;
            BornTime = Time.time;
            LastZoneId = -1;
            CoinRegistry.Register(this);   
        }

        private void OnDestroy()
        {
            CoinRegistry.Unregister(this); 
        }

        public void Multiply(int factor, float scatterImpulse = 0.8f, float upwardBias = 0.25f)
        {
            if (_markedForDestroy) return;
            if (Time.time - BornTime < spawnIgnoreTime) return;

            factor = Mathf.Max(1, factor);
            Vector3 pos = transform.position;

            for (int i = 0; i < factor; i++)
            {
                var cloneGO = Instantiate(gameObject, pos, Quaternion.identity);
                var clone   = cloneGO.GetComponent<Coin>();
                if (clone != null && clone.Body != null)
                {
                    Vector2 dir = new Vector2(Random.Range(-1f, 1f), Random.Range(0.15f, 1f)).normalized;
                    clone.Body.AddForce((dir + Vector2.up * upwardBias) * scatterImpulse, ForceMode2D.Impulse);
                    clone.Body.AddTorque(Random.Range(-30f, 30f));
                }
            }

            _markedForDestroy = true;
            Destroy(gameObject);
        }
    }
}
