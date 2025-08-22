using System.Collections;
using ArcadeHero2D.Core.CameraSys;
using ArcadeHero2D.Core.Flow;
using ArcadeHero2D.Core.Boot;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Minigame
{
    public sealed class CoinMiniGameController : MonoBehaviour
    {
        [SerializeField] CupDragController cupDrag;
        [SerializeField] CupPourController cupPour;
        [SerializeField] CoinCollector collector;
        [SerializeField] CameraRigController cameraRig;
        [SerializeField] float settleDelay = 0.8f;

        ICupBankService _cup;
        ICurrencyService _currency;

        public System.Action OnFinished;

        int _expectedCount;

        void Start()
        {
            _cup = ServiceLocator.Get<ICupBankService>();
            _currency = ServiceLocator.Get<ICurrencyService>();
        }

        public void StartMiniGame()
        {
            GameFlowController.Instance.SetPhase(GamePhase.CoinMiniGame);
            Time.timeScale = 1f;

            collector.ResetSum();
            cameraRig.MoveToBottom();

            _expectedCount = _cup.TakeAll();
            if (_expectedCount <= 0)
            {
                // Нечего сыпать — просто вернём камеру вверх и, когда она поднимется, завершим мини-игру
                StartCoroutine(FinishAfterCameraUp());
                return;
            }

            cupPour.OnAllSpawned = () => StartCoroutine(WaitForCollect());
            cupPour.Begin(_expectedCount);
        }

        IEnumerator WaitForCollect()
        {
            // дождались, пока досыпет все монеты
            while (cupPour.Pouring) yield return null;
            yield return new WaitForSeconds(settleDelay);

            // ждём сбор всех монет (или таймаут на всякий)
            float timeout = 12f;
            while (collector.CollectedCount < _expectedCount && timeout > 0f)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            // зачисляем в общий счёт
            _currency.Add(collector.TotalCollected);

            // поднимаем камеру
            yield return StartCoroutine(FinishAfterCameraUp());
        }

        IEnumerator FinishAfterCameraUp()
        {
            bool completed = false;
            System.Action handler = () => completed = true;

            cameraRig.OnMoveCompleted += handler;
            cameraRig.MoveToTop();

            // если кто-то вызвал MoveToTop ранее — подстрахуемся ожиданием по времени
            float guard = cameraRig.MoveTime + 0.1f;
            while (!completed && guard > 0f)
            {
                guard -= Time.deltaTime;
                yield return null;
            }

            cameraRig.OnMoveCompleted -= handler;

            // теперь камера гарантированно наверху — можно завершать мини-игру
            OnFinished?.Invoke();
        }
    }
}
