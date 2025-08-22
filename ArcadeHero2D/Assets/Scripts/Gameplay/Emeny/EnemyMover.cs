using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyMover : MonoBehaviour, IMovable
    {
        [SerializeField] private float speed = 1.2f;
        [SerializeField] private float stopDistance = 1.0f;
        [SerializeField] private bool lockToGroundY = true;

        private Transform _target;
        private float _laneY;
        private bool _stop;

        public void SetTarget(Transform t) => _target = t;

        public void SetLaneY(float y) => _laneY = y;

        private void Start()
        {
            if (lockToGroundY)
            {
                // если laneY не задан — зафиксируем текущую высоту как «землю»
                if (Mathf.Approximately(_laneY, 0f)) _laneY = transform.position.y;
                SnapY();
            }
        }

        private void Update()
        {
            if (_target == null || _stop) return;

            float dx = _target.position.x - transform.position.x;
            float absDx = Mathf.Abs(dx);

            if (absDx <= stopDistance)
            {
                _stop = true;
                SnapY();
                return;
            }

            float dirX = Mathf.Sign(dx);
            Move(new Vector2(dirX, 0f), speed);
            SnapY();
        }

        public void Move(Vector2 dir, float spd) =>
            transform.Translate(dir * spd * Time.deltaTime, Space.World);

        public void Stop() => _stop = true;

        private void SnapY()
        {
            if (!lockToGroundY) return;
            var p = transform.position;
            p.y = _laneY;
            transform.position = p;
        }
    }
}