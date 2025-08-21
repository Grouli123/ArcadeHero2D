using UnityEngine;
using UnityEngine.UI;

namespace ArcadeHero2D.UI.HUD
{
    public sealed class JourneyProgressBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Transform hero;
        private float _startX;
        private float _targetX;
        private bool _active;

        public void Bind(Transform heroT, float startX, float targetX)
        {
            hero = heroT;
            _startX = startX;
            _targetX = targetX;
            _active = true;
            slider.value = 0f;
        }

        private void Update()
        {
            if (!_active || hero == null) return;
            float v = Mathf.InverseLerp(_startX, _targetX, hero.position.x);
            slider.value = Mathf.Clamp01(v);
        }
    }
}