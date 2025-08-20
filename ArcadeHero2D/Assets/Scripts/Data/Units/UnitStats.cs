using UnityEngine;

namespace ArcadeHero2D.Data.Units
{
    [CreateAssetMenu(fileName = "UnitStats", menuName = "ArcadeHero2D/Unit Stats", order = 0)]
    public class UnitStats : ScriptableObject
    {
        public int maxHP = 10;
        public int attack = 2;
        public float attackRate = 1.2f;
        public float moveSpeed = 1f;
    }
}