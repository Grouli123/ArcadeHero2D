using ArcadeHero2D.Domain.Contracts;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeHero2D.UI.Bars
{
    public sealed class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private MonoBehaviour damageableProvider; // HeroController / EnemyController

        private IDamageable _damageable;
        private IHealth _health;

        private void Awake()
        {
            if (slider == null)
            {
                Debug.LogError($"{name}: Slider reference not set in HealthBarView!");
                enabled = false; return;
            }
            if (damageableProvider == null)
            {
                Debug.LogError($"{name}: DamageableProvider not set in HealthBarView!");
                enabled = false; return;
            }
            _damageable = damageableProvider as IDamageable;
            if (_damageable == null)
            {
                Debug.LogError($"{name}: DamageableProvider does not implement IDamageable!");
                enabled = false; return;
            }
        }

        private void Start()
        {
            // Ждём один кадр, чтобы UnitBase успел создать Health в своём Awake
            StartCoroutine(BindWhenReady());
        }

        private IEnumerator BindWhenReady()
        {
            // Подстраховка на случай редких гонок
            int safety = 10;
            while ((_health = _damageable.Health) == null && safety-- > 0)
                yield return null;

            if (_health == null)
            {
                Debug.LogError($"{name}: IDamageable.Health is still null after wait!");
                yield break;
            }

            slider.maxValue = _health.Max;
            slider.value = _health.Current;
            _health.OnChanged += OnHealthChanged;
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.OnChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(int current, int max)
        {
            slider.maxValue = max;
            slider.value = current;
        }
    }
}