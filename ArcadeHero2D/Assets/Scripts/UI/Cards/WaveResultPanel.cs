using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.UI
{
    public sealed class WaveResultPanel : MonoBehaviour
    {
        [SerializeField] Cards.CardView[] cardSlots;
        Canvas _canvas;
        IUpgradeService _upgrades;

        void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _upgrades = ServiceLocator.Get<IUpgradeService>();
            Hide();
        }

        public void Show()
        {
            var defs = _upgrades.Roll3();
            for (int i = 0; i < cardSlots.Length; i++)
                cardSlots[i].Bind(defs[i]);
            _canvas.enabled = true;
            Time.timeScale = 0f;
        }

        public void Hide()
        {
            _canvas.enabled = false;
            Time.timeScale = 1f;
        }
    }
}