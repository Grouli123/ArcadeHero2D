using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArcadeHero2D.Gameplay.Enemy;
using ArcadeHero2D.Gameplay.Hero;
using ArcadeHero2D.Gameplay.Projectiles;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Gameplay.Battle
{
    public sealed class BattleTurnController : MonoBehaviour
    {
        [SerializeField] private HeroAttackController heroAttack;
        [SerializeField] private Transform hero;
        [SerializeField] private float hitTimeout = 1.2f;

        public void Bind(HeroAttackController attack, Transform heroT)
        {
            heroAttack = attack;
            hero = heroT;
        }

        public IEnumerator RunBattle(IList<EnemyController> enemies)
        {
            enemies = enemies.Where(e => e != null).ToList();

            while (enemies.Count > 0 && IsHeroAlive())
            {
                var target = ChooseTarget(enemies);
                if (target == null) yield break;

                // ждём КД героя
                while (!heroAttack.CanAttack) yield return null;

                bool hit = false;
                void OnHit(EnemyController e, int dmg)
                {
                    if (e == target) hit = true;
                }
                ArcProjectile.OnHeroHitEnemy += OnHit;

                heroAttack.TryAttack(target.transform);

                float t = hitTimeout;
                while (!hit && (t -= Time.deltaTime) > 0f) yield return null;
                ArcProjectile.OnHeroHitEnemy -= OnHit;

                // Ход врагов: каждый делает шаг/выстрел
                for (int i = 0; i < enemies.Count; i++)
                {
                    var e = enemies[i];
                    if (e == null) continue;
                    var responders = e.GetComponents<IEnemyResponder>();
                    for (int r = 0; r < responders.Length; r++)
                        responders[r].OnHeroAttacked();
                }

                // чистим мёртвых
                for (int i = enemies.Count - 1; i >= 0; i--)
                    if (enemies[i] == null || !enemies[i].IsAlive)
                        enemies.RemoveAt(i);

                yield return null;
            }
        }

        EnemyController ChooseTarget(IList<EnemyController> enemies)
        {
            EnemyController best = null;
            float bestX = float.MaxValue;
            for (int i = 0; i < enemies.Count; i++)
            {
                var e = enemies[i];
                if (e == null || !e.IsAlive) continue;
                float ex = e.transform.position.x;
                if (ex < bestX) { bestX = ex; best = e; }
            }
            return best;
        }

        bool IsHeroAlive()
        {
            return hero != null && hero.TryGetComponent<IDamageable>(out var d) && d.IsAlive;
        }
    }
}
