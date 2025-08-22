using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public sealed class SlotMover : MonoBehaviour
    {
        [SerializeField] private float approachSpeed = 2.0f;
        [SerializeField] private float reachEps = 0.02f;
        [SerializeField] private bool lockToLaneY = true;

        public float slotX;
        private float _laneY;
        private bool _hasSlot;

        public bool HasSlot => _hasSlot;
        public bool InSlot { get; private set; }

        public void SetLaneY(float y) => _laneY = y;

        public void SetSlotX(float x)
        {
            slotX = x;
            _hasSlot = true;
            InSlot = false;
        }

        public void ForceSnapToSlot()
        {
            if (!_hasSlot) return;
            var p = transform.position;
            p.x = slotX;
            if (lockToLaneY) p.y = _laneY;
            transform.position = p;
            InSlot = true;
        }

        private void Update()
        {
            if (!_hasSlot || InSlot) return;

            var p = transform.position;
            if (lockToLaneY) p.y = _laneY;

            float dx = slotX - p.x;
            float adx = Mathf.Abs(dx);

            if (adx <= reachEps)
            {
                p.x = slotX;
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