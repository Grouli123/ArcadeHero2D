using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Contracts;
using TMPro;
using UnityEngine;

namespace ArcadeHero2D.UI.HUD
{
    public sealed class StatValueView : MonoBehaviour
    {
        public enum StatKind { Attack, AttackSpeed }

        [SerializeField] StatKind kind;
        [SerializeField] TMP_Text label;
        IStatsService _stats;

        void Awake()
        {
            _stats = ServiceLocator.Get<IStatsService>();
            _stats.OnChanged += Refresh;
            Refresh();
        }

        void Refresh()
        {
            label.text = kind == StatKind.Attack
                ? _stats.Attack.ToString()
                : _stats.AttackSpeed.ToString("0.0");
        }
    }
}