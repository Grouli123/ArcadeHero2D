using UnityEngine;

namespace ArcadeHero2D.Data.Upgrades
{
    [CreateAssetMenu(fileName = "UpgradeDefinition", menuName = "ArcadeHero2D/Upgrade Definition", order = 0)]
    public class UpgradeDefinition : ScriptableObject
    {
        public string title;
        public Sprite icon;
        public int price;

        public StatType statType;
        public int intValue;
        public float floatValue;
    }

    public enum StatType
    {
        Attack,
        AttackSpeed,
        MaxHP
    }
}