using UnityEngine;

namespace ArcadeHero2D.Domain.Contracts
{
    public interface IEnemyResponder
    {
        void Init(Transform hero);
        void OnHeroHit();
    }
}