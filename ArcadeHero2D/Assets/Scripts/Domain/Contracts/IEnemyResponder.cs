using UnityEngine;

namespace ArcadeHero2D.Gameplay.Enemy
{
    public interface IEnemyResponder
    {
        void Init(Transform hero);
        void OnHeroAttacked();
    }
}