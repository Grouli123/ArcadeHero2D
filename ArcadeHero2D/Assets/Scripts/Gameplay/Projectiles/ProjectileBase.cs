using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Projectiles
{
    public abstract class ProjectileBase : MonoBehaviour, IProjectile
    {
        protected int _damage;
        public abstract void Launch(Vector2 origin, Vector2 target, int damage);
    }
}