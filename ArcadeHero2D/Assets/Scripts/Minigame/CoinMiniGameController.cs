using System.Collections;
using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Core.CameraSys;
using ArcadeHero2D.Core.Flow;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    public sealed class CoinMiniGameController : MonoBehaviour
    {
        [SerializeField] private CupDragController cupDrag;
        [SerializeField] private CupPourController cupPour;
        [SerializeField] private CoinCollector collector;
        [SerializeField] private CameraRigController cameraRig;
        [SerializeField] private float settleDelay = 1.0f;

        private ICupBankService _cup;
        private ICurrencyService _currency;
        public System.Action OnFinished;

        private void Start()
        {
            _cup = ServiceLocator.Get<ICupBankService>();
            _currency = ServiceLocator.Get<ICurrencyService>();
            if (_cup == null || _currency == null)
            {
                Debug.LogError($"{name}: CoinMiniGameController — services not registered");
                enabled = false; return;
            }
        }

        public void StartMiniGame()
        {
            if (!enabled) return;
            GameFlowController.Instance.SetPhase(GamePhase.CoinMiniGame);
            collector.ResetSum();
            cameraRig.MoveToBottom();
            int count = _cup.TakeAll();
            cupPour.OnAllSpawned = () => StartCoroutine(WaitForSettle());
            cupPour.Begin(count);
        }

        IEnumerator WaitForSettle()
        {
            yield return new WaitForSeconds(settleDelay);
            yield return new WaitForEndOfFrame();
            _currency.Add(collector.TotalCollected);
            cameraRig.MoveToTop();
            OnFinished?.Invoke();
        }
    }
}