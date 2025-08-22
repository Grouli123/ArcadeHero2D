using ArcadeHero2D.Core.Flow;
using UnityEngine;

namespace ArcadeHero2D.UI
{
    public sealed class MenuStartController : MonoBehaviour
    {
        [SerializeField] private string preloadScene  = "Preload";
        [SerializeField] private string gameplayScene = "Gameplay";

        public void OnStartPressed()
        {
            Time.timeScale = 1f; // на всякий
            SceneLoadFlow.LoadViaPreload(gameplayScene, preloadScene);
        }

        public void OnQuitPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}