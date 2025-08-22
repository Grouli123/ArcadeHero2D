using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Hero
{
    public sealed class HeroMover : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 1.5f;
        bool _moving;
        public void Resume() => _moving = true;
        public void Stop()   => _moving = false;

        void Update()
        {
            if (_moving)
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}