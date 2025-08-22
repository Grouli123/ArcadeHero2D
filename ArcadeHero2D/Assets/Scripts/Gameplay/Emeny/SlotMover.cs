using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class SlotMover : MonoBehaviour
    {
        [SerializeField] float approachSpeed = 2.0f;
        [SerializeField] float reachEps = 0.02f;
        [SerializeField] bool lockToLaneY = true;

        float _slotX;
        float _laneY;
        bool _hasSlot;

        public bool HasSlot => _hasSlot;
        public bool InSlot { get; private set; }

        public void SetLaneY(float y) => _laneY = y;

        public void SetSlotX(float x)
        {
            _slotX = x;
            _hasSlot = true;
            InSlot = false;
        }

        public void ForceSnapToSlot()
        {
            if (!_hasSlot) return;
            var p = transform.position;
            p.x = _slotX;
            if (lockToLaneY) p.y = _laneY;
            transform.position = p;
            InSlot = true;
        }

        void Update()
        {
            if (!_hasSlot || InSlot) return;

            var p = transform.position;
            if (lockToLaneY) p.y = _laneY;

            float dx = _slotX - p.x;
            float adx = Mathf.Abs(dx);

            if (adx <= reachEps)
            {
                p.x = _slotX;
                transform.position = p;
                InSlot = true;
                return;
            }

            float dir = Mathf.Sign(dx);
            p.x += dir * approachSpeed * Time.deltaTime;
            transform.position = p;
        }
    }
}