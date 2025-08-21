using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class EnemyMover : MonoBehaviour, IMovable
    {
        [SerializeField] float speed = 1.2f;
        [SerializeField] float stopDistance = 1.0f;
        [SerializeField] bool lockToGroundY = true;

        Transform _target;
        float _laneY;
        bool _stop;

        public void SetTarget(Transform t) => _target = t;

        // Задаём «линию» (высоту) движения — вызовем из EnemyController.Init(hero)
        public void SetLaneY(float y) => _laneY = y;

        void Start()
        {
            if (lockToGroundY)
            {
                // если laneY не задан — зафиксируем текущую высоту как «землю»
                if (Mathf.Approximately(_laneY, 0f)) _laneY = transform.position.y;
                SnapY();
            }
        }

        void Update()
        {
            if (_target == null || _stop) return;

            // считаем горизонтальную дистанцию
            float dx = _target.position.x - transform.position.x;
            float absDx = Mathf.Abs(dx);

            if (absDx <= stopDistance)
            {
                _stop = true;
                SnapY();
                return;
            }

            // двигаемся ТОЛЬКО по X
            float dirX = Mathf.Sign(dx);
            Move(new Vector2(dirX, 0f), speed);
            SnapY();
        }

        public void Move(Vector2 dir, float spd) =>
            transform.Translate(dir * spd * Time.deltaTime, Space.World);

        public void Stop() => _stop = true;

        void SnapY()
        {
            if (!lockToGroundY) return;
            var p = transform.position;
            p.y = _laneY;
            transform.position = p;
        }
    }
}