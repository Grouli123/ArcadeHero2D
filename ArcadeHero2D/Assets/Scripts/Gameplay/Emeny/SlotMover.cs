using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class SlotMover : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] private float moveSpeed = 2.2f;
        [SerializeField] private float stopEpsilon = 0.02f;
        [SerializeField] private float slowDownDistance = 0.25f;

        [Header("Lane")]
        [SerializeField] private bool lockToLaneY = true;

        private Rigidbody2D _rb;
        private float? _targetX;
        private float  _laneY;

        public bool InSlot => !_targetX.HasValue;
        public float? TargetX => _targetX;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

            if (lockToLaneY) _laneY = _rb.position.y;
        }

        public void SetLaneY(float y)
        {
            _laneY = y;
            if (lockToLaneY)
            {
                var p = _rb.position;
                p.y = y;
                _rb.position = p; 
            }
        }

        public void SetSlotX(float slotX) => SetTargetX(slotX); 
        public void SetTargetX(float x)    => _targetX = x;
        public void ClearTarget()          => _targetX = null;

        private void FixedUpdate()
        {
            if (!_targetX.HasValue) return;

            float x   = _rb.position.x;
            float tx  = _targetX.Value;
            float dx  = tx - x;
            float adx = Mathf.Abs(dx);

            if (adx <= stopEpsilon)
            {
                _rb.MovePosition(new Vector2(tx, lockToLaneY ? _laneY : _rb.position.y));
                _targetX = null;
                return;
            }

            float dir   = Mathf.Sign(dx);
            float speed = moveSpeed;

            if (adx < slowDownDistance)
            {
                float t = Mathf.InverseLerp(0f, slowDownDistance, adx);
                speed = Mathf.Lerp(0.1f, moveSpeed, t);
            }

            float step = dir * speed * Time.fixedDeltaTime;
            float newX = x + step;

            if (Mathf.Abs(tx - newX) > adx) newX = tx;

            var p = new Vector2(newX, lockToLaneY ? _laneY : _rb.position.y);
            _rb.MovePosition(p);
        }
    }
}