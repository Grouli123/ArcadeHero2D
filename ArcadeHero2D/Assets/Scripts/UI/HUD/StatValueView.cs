using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Contracts;
using TMPro;
using UnityEngine;

namespace ArcadeHero2D.UI.HUD
{
    public sealed class StatValueView : MonoBehaviour
    {
        public enum StatKind { Attack, AttackSpeed }

        [SerializeField] private StatKind kind;
        [SerializeField] private TMP_Text label;
        IStatsService _stats;

        private void Awake()
        {
            _stats = ServiceLocator.Get<IStatsService>();
            _stats.OnChanged += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            label.text = kind == StatKind.Attack
                ? _stats.Attack.ToString()
                : _stats.AttackSpeed.ToString("0.0");
        }
    }
}