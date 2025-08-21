using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Hero
{
    public sealed class HeroMover : MonoBehaviour, IMovable
    {
        [field: SerializeField] public float MoveSpeed { get; private set; } = 1.5f;
        bool _stopped;

        public void Move(Vector2 direction, float speed)
        {
            if (_stopped) return;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }

        public void Stop() => _stopped = true;
        public void Resume() => _stopped = false;
    }
}