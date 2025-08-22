using System.Collections;
using System;
using UnityEngine;

namespace ArcadeHero2D.Core.CameraSys
{
    public sealed class CameraRigController : MonoBehaviour
    {
        [SerializeField] private Transform rig;
        [SerializeField] private Transform topAnchor;
        [SerializeField] private Transform bottomAnchor;
        [SerializeField] private float moveTime = 0.7f;

        public float MoveTime => moveTime;
        public bool IsMoving { get; private set; }
        public event Action OnMoveCompleted;

        public void MoveToTop()    => StartCoroutine(MoveTo(topAnchor.position));
        public void MoveToBottom() => StartCoroutine(MoveTo(bottomAnchor.position));

        private IEnumerator MoveTo(Vector3 target)
        {
            if (rig == null) rig = transform;
            Vector3 start = rig.position;
            float t = 0f;
            IsMoving = true;

            while (t < moveTime)
            {
                t += Time.deltaTime;
                float k = Mathf.SmoothStep(0f, 1f, t / moveTime);
                rig.position = Vector3.Lerp(start, target, k);
                yield return null;
            }
            rig.position = target;
            IsMoving = false;
            OnMoveCompleted?.Invoke();
        }
    }
}