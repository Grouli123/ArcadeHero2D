using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Data.Upgrades;
using ArcadeHero2D.Domain.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeHero2D.UI.Cards
{
    public sealed class CardView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text price;
        private UpgradeDefinition _def;
        private IUpgradeService _upgrades;
        private UI.WaveResultPanel _owner;

        private void Awake() => _upgrades = ServiceLocator.Get<IUpgradeService>();

        public void Bind(UpgradeDefinition def, UI.WaveResultPanel owner)
        {
            _def = def;
            _owner = owner;
            if (icon) icon.sprite = def.icon;
            title.text = def.title;
            price.text = def.price.ToString();
        }

        public void OnClick()
        {
            if (_def == null) return;
            if (_upgrades.TryApply(_def))
                _owner?.NotifyChosen();
        }
    }
}