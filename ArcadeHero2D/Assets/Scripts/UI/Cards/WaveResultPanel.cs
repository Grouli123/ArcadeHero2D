using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.UI
{
    public sealed class WaveResultPanel : MonoBehaviour
    {
        [SerializeField] private Cards.CardView[] cardSlots;
        private Canvas _canvas;
        private IUpgradeService _upgrades;
        public System.Action OnUpgradeChosen;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _upgrades = ServiceLocator.Get<IUpgradeService>();
            Hide();
        }

        public void Show()
        {
            var defs = _upgrades.Roll3();
            for (int i = 0; i < cardSlots.Length; i++)
                cardSlots[i].Bind(defs[i], this);
            _canvas.enabled = true;
            Time.timeScale = 0f;
        }

        public void Hide()
        {
            _canvas.enabled = false;
            Time.timeScale = 1f;
        }

        public void NotifyChosen()
        {
            Hide();
            OnUpgradeChosen?.Invoke();
        }
    }
}