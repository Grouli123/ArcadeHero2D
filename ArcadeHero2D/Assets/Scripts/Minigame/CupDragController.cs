using System;
using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    public sealed class CupDragController : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] private float dragLerp = 12f;
        [SerializeField] private float minX = -3.5f;
        [SerializeField] private float maxX =  3.5f;

        [Header("Input")]
        [SerializeField] private bool acceptAnywhere = true; 

        public event Action OnUserInteract; 

        private bool _interactable;
        private bool _isPointerDown;
        private bool _interactionSent;
        private Camera _cam;

        public void EnableInput(bool enabled)
        {
            _interactable = enabled;
            _isPointerDown = false;
            _interactionSent = false;
        }

        private void Awake()
        {
            _cam = Camera.main;
            EnableInput(false);
        }

        private void Update()
        {
            if (!_interactable) return;

            bool pointerDownNow = Input.GetMouseButton(0) || Input.touchCount > 0;

            if (pointerDownNow && !_isPointerDown && !_interactionSent)
            {
                _interactionSent = true;
                OnUserInteract?.Invoke();
            }
            _isPointerDown = pointerDownNow;

            if (!_isPointerDown) return;

            Vector3 m = Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
            var w = _cam != null ? _cam.ScreenToWorldPoint(m) : m;
            float targetX = Mathf.Clamp(w.x, minX, maxX);

            var p = transform.position;
            p.x = Mathf.Lerp(p.x, targetX, 1f - Mathf.Exp(-dragLerp * Time.deltaTime));
            transform.position = p;
        }
    }
}