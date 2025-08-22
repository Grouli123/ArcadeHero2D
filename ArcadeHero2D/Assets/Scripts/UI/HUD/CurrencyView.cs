using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Contracts;
using TMPro;
using UnityEngine;

namespace ArcadeHero2D.UI.HUD
{
    public sealed class CurrencyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        private ICurrencyService _currency;

        private void Awake()
        {
            _currency = ServiceLocator.Get<ICurrencyService>();
            _currency.OnChanged += UpdateView;
            UpdateView(_currency.Soft);
        }

        private void UpdateView(int value) => label.text = value.ToString();
    }
}