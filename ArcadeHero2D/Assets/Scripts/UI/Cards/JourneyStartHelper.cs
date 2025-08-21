using UnityEngine;
using UnityEngine.Events;

namespace ArcadeHero2D.UI.Cards
{
    public sealed class JourneyStartHelper : MonoBehaviour
    {
        [SerializeField] private UnityEvent onUpgradeChosen;
        public void OnChosen() => onUpgradeChosen?.Invoke();
    }
}