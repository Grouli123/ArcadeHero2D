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
        [SerializeField] Image icon;
        [SerializeField] TMP_Text title;
        [SerializeField] TMP_Text price;
        UpgradeDefinition _def;
        IUpgradeService _upgrades;

        void Awake() => _upgrades = ServiceLocator.Get<IUpgradeService>();

        public void Bind(UpgradeDefinition def)
        {
            _def = def;
            if (icon) icon.sprite = def.icon;
            title.text = def.title;
            price.text = def.price.ToString();
        }

        public void OnClick()
        {
            if (_def == null) return;
            if (_upgrades.TryApply(_def))
            {
                transform.root.SendMessage("Hide", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}