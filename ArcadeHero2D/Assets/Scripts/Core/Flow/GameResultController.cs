using System.Collections;
using ArcadeHero2D.Core.Flow;
using ArcadeHero2D.Domain.Base;
using ArcadeHero2D.Domain.Contracts;   
using ArcadeHero2D.Rendering;
using ArcadeHero2D.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using ArcadeHero2D.Gameplay.Waves;

namespace ArcadeHero2D.Core.Systems
{
    public sealed class GameResultController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private EndGamePanel panel;
        [SerializeField] private Transform hero;           
        [SerializeField] private WaveSequenceController waveSequence; 

        [Header("Options")]
        [SerializeField] private string menuSceneName = "";  
        [SerializeField] private bool pauseOnShow = true;

        private UnitBase _heroUnit;
        private IHealth _heroHealth;
        private bool _shown;

        private void Awake()
        {
            if (hero)
            {
                _heroUnit = hero.GetComponent<UnitBase>();
                if (_heroUnit == null) _heroUnit = hero.GetComponentInChildren<UnitBase>();
            }
            if (_heroUnit != null)
                _heroHealth = _heroUnit.Health;
        }

        private void OnEnable()
        {
            if (_heroHealth != null) _heroHealth.OnDied += OnHeroDied;
            if (waveSequence != null) waveSequence.OnSequenceCompleted += OnVictory;
        }

        private void OnDisable()
        {
            if (_heroHealth != null) _heroHealth.OnDied -= OnHeroDied;
            if (waveSequence != null) waveSequence.OnSequenceCompleted -= OnVictory;
        }

        private void OnHeroDied()
        {
            if (_shown) return;
            StartCoroutine(ShowAfterDeath());
        }

        private IEnumerator ShowAfterDeath()
        {
            // подождём анимацию смерти, если есть
            float wait = 0.6f;
            var anim = hero ? hero.GetComponentInChildren<UnitAnimationController>() : null;
            if (anim != null) wait = anim.GetDeathDuration() + 0.15f;
            yield return new WaitForSeconds(wait);

            Show(GameResult.Defeat);
        }

        private void OnVictory()
        {
            if (_shown) return;
            Show(GameResult.Victory);
        }

        private void Show(GameResult result)
        {
            _shown = true;
            if (pauseOnShow) Time.timeScale = 0f;

            string extra = result == GameResult.Victory ? "Все волны пройдены!" : "Попробуйте снова.";
            panel.Show(result, extra, OnRestart, OnMenu);
        }

        private void OnRestart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnMenu()
        {
            Time.timeScale = 1f;
            if (!string.IsNullOrEmpty(menuSceneName))
                SceneManager.LoadScene(menuSceneName);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
