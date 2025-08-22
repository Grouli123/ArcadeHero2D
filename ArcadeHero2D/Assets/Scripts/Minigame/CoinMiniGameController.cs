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
        [SerializeField] GameObject cupRoot;
        [SerializeField] CupDragController cupDrag;
        [SerializeField] CupPourController cupPour;
        [SerializeField] CoinCollector collector;
        [SerializeField] CameraRigController cameraRig;

        [Header("Tuning")]
        [SerializeField] float settleDelay = 0.8f;
        [SerializeField] float slowCoinsPerSecond = 4f;
        [SerializeField] bool globalTapStartsPour = false;

        ICupBankService  _cup;
        ICurrencyService _currency;

        int  _expectedCount;
        bool _cameraDown;
        bool _pourStarted;

        public System.Action OnFinished;

        void Awake()
        {
            _cup      = ServiceLocator.Get<ICupBankService>();
            _currency = ServiceLocator.Get<ICurrencyService>();
            SetCupVisible(false);
        }

        void OnEnable()
        {
            if (cupDrag != null) cupDrag.OnUserInteract += HandleUserInteract;
        }

        void OnDisable()
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

            // перед стартом — сбросить реестр живых монет (никаких старых хвостов)
            CoinRegistry.Reset();

            _expectedCount = _cup != null ? _cup.TakeAll() : 0;

            cameraRig.OnMoveCompleted += OnCamDownComplete;
            cameraRig.MoveToBottom();
        }

        void OnCamDownComplete()
        {
            cameraRig.OnMoveCompleted -= OnCamDownComplete;
            _cameraDown = true;

            if (_expectedCount <= 0)
            {
                // Монет нет — пустой цикл: просто наверх после «вниз»
                StartCoroutine(FinishAfterCameraUp(hideCupBeforeMoveUp:false));
                return;
            }

            SetCupVisible(true);
        }

        void HandleUserInteract()
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

        void Update()
        {
            if (!_cameraDown || _pourStarted || !globalTapStartsPour) return;
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
                HandleUserInteract();
        }

        IEnumerator WaitForAllCoinsCollected()
        {
            // ждём, пока монеты досыпятся
            while (cupPour != null && cupPour.Pouring) yield return null;

            // ещё чуть-чуть, чтобы физика успокоилась
            yield return new WaitForSeconds(settleDelay);

            // ключевое ожидание: пока в сцене есть живые монеты — стоим
            while (CoinRegistry.Live > 0)
                yield return null;

            // все монеты уже упали в стакан и уничтожены — значит collector собрал финальную сумму
            if (_currency != null) _currency.Add(collector.TotalCollected);

            // кадр на обновление UI (верхний бар)
            yield return null;

            // прячем стакан и поднимаем камеру; карточки покажутся только после полного подъёма
            yield return StartCoroutine(FinishAfterCameraUp(hideCupBeforeMoveUp:true));
        }

        IEnumerator FinishAfterCameraUp(bool hideCupBeforeMoveUp)
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

        void SetCupVisible(bool visible)
        {
            if (cupRoot != null) cupRoot.SetActive(visible);
            if (cupDrag != null) cupDrag.EnableInput(visible);
        }
    }
}
