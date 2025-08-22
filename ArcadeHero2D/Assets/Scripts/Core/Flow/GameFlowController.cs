using System;
using UnityEngine;

namespace ArcadeHero2D.Core.Flow
{
    public enum GamePhase { Journey, Battle, CoinMiniGame, Upgrade }

    public sealed class GameFlowController : MonoBehaviour
    {
        public static GameFlowController Instance { get; private set; }

        public GamePhase Phase { get; private set; } = GamePhase.Journey;
        public event Action<GamePhase> OnPhaseChanged;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetPhase(GamePhase phase)
        {
            if (Phase == phase) return;
            Phase = phase;
            OnPhaseChanged?.Invoke(Phase);
        }
    }
}