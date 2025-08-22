using UnityEngine;

namespace ArcadeHero2D.Gameplay.Hero
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class HeroMover : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1.5f;
        private Rigidbody2D _rb;
        private bool _moving;

        public void Resume() => _moving = true;
        public void Stop()   => _moving = false;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        }

        private void FixedUpdate()
        {
            if (!_moving) return;
            var p = _rb.position;
            p.x += moveSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(p);
        }
    }
}