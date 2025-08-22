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
        [Header("Refs")]
        [SerializeField] private GameObject cupRoot;
        [SerializeField] private CupDragController cupDrag;
        [SerializeField] private CupPourController cupPour;
        [SerializeField] private CoinCollector collector;
        [SerializeField] private CameraRigController cameraRig;

        [Header("Tuning")]
        [SerializeField] private float settleDelay = 0.8f;
        [SerializeField] private float slowCoinsPerSecond = 4f;
        [SerializeField] private bool globalTapStartsPour = false;

        private ICupBankService  _cup;
        private ICurrencyService _currency;

        private int  _expectedCount;
        private bool _cameraDown;
        private bool _pourStarted;

        public System.Action OnFinished;

        private void Awake()
        {
            _cup      = ServiceLocator.Get<ICupBankService>();
            _currency = ServiceLocator.Get<ICurrencyService>();
            SetCupVisible(false);
        }
        
        private void OnEnable()
        {
            if (cupDrag != null) cupDrag.OnUserInteract += HandleUserInteract;
        }

        private void OnDisable()
        {
            if (cupDrag != null) cupDrag.OnUserInteract -= HandleUserInteract;
        }

        public void StartMiniGame()
        {
            GameFlowController.Instance.SetPhase(GamePhase.CoinMiniGame);

            _cameraDown  = false;
            _pourStarted = false;
            collector.ResetSum();
            SetCupVisible(false);

            CoinRegistry.Reset();

            _expectedCount = _cup != null ? _cup.TakeAll() : 0;

            cameraRig.OnMoveCompleted += OnCamDownComplete;
            cameraRig.MoveToBottom();
        }

        private void OnCamDownComplete()
        {
            cameraRig.OnMoveCompleted -= OnCamDownComplete;
            _cameraDown = true;

            if (_expectedCount <= 0)
            {
                StartCoroutine(FinishAfterCameraUp(hideCupBeforeMoveUp:false));
                return;
            }

            SetCupVisible(true);
        }

        private void HandleUserInteract()
        {
            if (!_cameraDown || _pourStarted) return;
            _pourStarted = true;

            if (cupPour != null)
            {
                cupPour.ConfigureRate(slowCoinsPerSecond);
                cupPour.OnAllSpawned = () => StartCoroutine(WaitForAllCoinsCollected());
                cupPour.Begin(_expectedCount);
            }
        }

        private void Update()
        {
            if (!_cameraDown || _pourStarted || !globalTapStartsPour) return;
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                HandleUserInteract();
        }

        private IEnumerator WaitForAllCoinsCollected()
        {
            while (cupPour != null && cupPour.Pouring) yield return null;

            yield return new WaitForSeconds(settleDelay);

            while (CoinRegistry.Live > 0)
                yield return null;

            if (_currency != null) _currency.Add(collector.TotalCollected);

            yield return null;

            yield return StartCoroutine(FinishAfterCameraUp(hideCupBeforeMoveUp:true));
        }

        private IEnumerator FinishAfterCameraUp(bool hideCupBeforeMoveUp)
        {
            if (hideCupBeforeMoveUp)
                SetCupVisible(false);

            bool completed = false;
            System.Action handler = () => completed = true;
            cameraRig.OnMoveCompleted += handler;
            cameraRig.MoveToTop();

            float guard = cameraRig != null ? cameraRig.MoveTime + 0.2f : 1f;
            while (!completed && guard > 0f)
            {
                guard -= Time.deltaTime;
                yield return null;
            }
            cameraRig.OnMoveCompleted -= handler;

            OnFinished?.Invoke();
        }

        private void SetCupVisible(bool visible)
        {
            if (cupRoot != null) cupRoot.SetActive(visible);
            if (cupDrag != null) cupDrag.EnableInput(visible);
        }
    }
}
