using System.Collections;
using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    public sealed class CupPourController : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Coin coinPrefab;
        [SerializeField] private float interval = 0.05f;
        [SerializeField] private float initialImpulse = 0.5f;

        public int ToSpawn { get; private set; }
        public int Spawned { get; private set; }
        public bool Pouring { get; private set; }
        public System.Action OnAllSpawned;

        public void Begin(int count)
        {
            if (Pouring) return;
            ToSpawn = Mathf.Max(0, count);
            Spawned = 0;
            StartCoroutine(Pour());
        }

        IEnumerator Pour()
        {
            Pouring = true;
            while (Spawned < ToSpawn)
            {
                var c = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity);
                c.SetValue(1);
                c.Body.AddForce(Vector2.right * Random.Range(0.05f, 0.15f) + Vector2.up * initialImpulse, ForceMode2D.Impulse);
                Spawned++;
                yield return new WaitForSeconds(interval);
            }
            Pouring = false;
            OnAllSpawned?.Invoke();
        }
    }
}