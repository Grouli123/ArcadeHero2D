using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class EnemyMover : MonoBehaviour
    {
        [SerializeField] private float speed = 1.2f;
        [SerializeField] private float stopDistance = 1.0f;
        [SerializeField] private bool lockToGroundY = true;

        private Transform _target;
        private Rigidbody2D _rb;
        private float _laneY;
        private bool _stop;

        public void SetTarget(Transform t) => _target = t;
        public void SetLaneY(float y) => _laneY = y;
        public void Stop() => _stop = true;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation | (lockToGroundY ? RigidbodyConstraints2D.FreezePositionY : RigidbodyConstraints2D.None);
            if (lockToGroundY) _laneY = transform.position.y;
        }

        private void FixedUpdate()
        {
            if (_stop || _target == null) return;

            float dx = _target.position.x - _rb.position.x;
            float adx = Mathf.Abs(dx);
            if (adx <= stopDistance) return;

            float dir = Mathf.Sign(dx);
            var p = _rb.position;
            p.x += dir * speed * Time.fixedDeltaTime;
            if (lockToGroundY) p.y = _laneY;
            _rb.MovePosition(p);
        }
    }
}