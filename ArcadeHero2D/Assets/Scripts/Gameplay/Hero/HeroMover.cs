using ArcadeHero2D.Rendering;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Hero
{
    public sealed class HeroMover : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 1.5f;
        private bool _moving;
        private UnitAnimationController _anim;

        private void Awake() => _anim = GetComponentInChildren<UnitAnimationController>();

        public void Resume()
        {
            _moving = true;
            if (_anim) _anim.SetMoving(true);
        }

        public void Stop()
        {
            _moving = false;
            if (_anim) _anim.SetMoving(false);
        }

        private void Update()
        {
            if (_moving)
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}