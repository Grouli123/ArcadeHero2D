using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ArcadeHero2D.Core.Flow
{
    public sealed class PreloadController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Slider  progressBar;
        [SerializeField] private TMP_Text percentLabel;

        [Header("Settings")]
        [SerializeField] private string fallbackTargetScene = "Gameplay";
        [SerializeField] private float minShowTime = 1.0f;       
        [SerializeField] private float activateDelay = 0.05f;    

        private void Start()
        {
            Time.timeScale = 1f; 
            StartCoroutine(LoadRoutine());
        }

        private IEnumerator LoadRoutine()
        {
            string target = SceneLoadFlow.ConsumeTargetOr(fallbackTargetScene);

            SetProgress(0f);
            yield return null;

            var op = SceneManager.LoadSceneAsync(target);
            op.allowSceneActivation = false;

            float shown = 0f;

            while (op.progress < 0.9f)
            {
                float p = Mathf.Clamp01(op.progress / 0.9f); 
                SetProgress(Mathf.Lerp(progressBar ? progressBar.value : 0f, p * 0.95f, 0.25f));
                shown += Time.unscaledDeltaTime;
                yield return null;
            }

            while (shown < minShowTime)
            {
                shown += Time.unscaledDeltaTime;
                SetProgress(Mathf.Lerp(progressBar ? progressBar.value : 0f, 0.98f, 0.25f));
                yield return null;
            }

            SetProgress(1f);
            yield return new WaitForSecondsRealtime(activateDelay);
            op.allowSceneActivation = true;
        }

        private void SetProgress(float v)
        {
            if (progressBar) progressBar.value = Mathf.Clamp01(v);
            if (percentLabel) percentLabel.text = Mathf.RoundToInt(Mathf.Clamp01(v) * 100f) + "%";
        }
    }
}