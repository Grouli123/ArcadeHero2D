using System;
using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    public sealed class CupDragController : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] float dragLerp = 12f;
        [SerializeField] float minX = -3.5f;
        [SerializeField] float maxX =  3.5f;

        [Header("Input")]
        [SerializeField] bool acceptAnywhere = true; // двигать по любому тапу (а не только по коллайдеру)

        public event Action OnUserInteract; // первый тап/удержание после EnableInput(true)

        bool _interactable;
        bool _isPointerDown;
        bool _interactionSent;
        Camera _cam;

        public void EnableInput(bool enabled)
        {
            _interactable = enabled;
            _isPointerDown = false;
            _interactionSent = false;
        }

        void Awake()
        {
            _cam = Camera.main;
            EnableInput(false);
        }

        void Update()
        {
            if (!_interactable) return;

            // pointer down / up (мышь или тач)
            bool pointerDownNow = Input.GetMouseButton(0) || Input.touchCount > 0;

            // первый кадр нажатия — шлём событие
            if (pointerDownNow && !_isPointerDown && !_interactionSent)
            {
                _interactionSent = true;
                OnUserInteract?.Invoke();
            }
            _isPointerDown = pointerDownNow;

            if (!_isPointerDown) return;

            // вычисляем целевой X из курсора/тача
            Vector3 m = Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
            var w = _cam != null ? _cam.ScreenToWorldPoint(m) : m;
            float targetX = Mathf.Clamp(w.x, minX, maxX);

            // сглаженное перемещение
            var p = transform.position;
            p.x = Mathf.Lerp(p.x, targetX, 1f - Mathf.Exp(-dragLerp * Time.deltaTime));
            transform.position = p;
        }

        // Если хочешь перемещать только при попадании по самому стакану — поставь acceptAnywhere = false
        // и раскомментируй OnMouseDown/OnMouseUp ниже, плюс доп. логику для drag:
        /*
        void OnMouseDown()
        {
            if (!_interactable || acceptAnywhere) return;
            _isPointerDown = true;
            if (!_interactionSent) { _interactionSent = true; OnUserInteract?.Invoke(); }
        }
        void OnMouseUp() { if (!acceptAnywhere) _isPointerDown = false; }
        */
    }
}
