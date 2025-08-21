using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    public sealed class CupDragController : MonoBehaviour
    {
        [SerializeField] private float minX = -3f;
        [SerializeField] private float maxX = 3f;
        [SerializeField] private float dragSpeed = 10f;
        private Camera _cam;
        private bool _drag;

        private void Awake() => _cam = Camera.main;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) _drag = true;
            if (Input.GetMouseButtonUp(0)) _drag = false;
            if (!_drag) return;
            var pos = _cam.ScreenToWorldPoint(Input.mousePosition);
            float x = Mathf.Clamp(pos.x, minX, maxX);
            var p = transform.position;
            p.x = Mathf.Lerp(p.x, x, Time.deltaTime * dragSpeed);
            transform.position = p;
        }
    }
}