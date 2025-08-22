using UnityEngine.SceneManagement;

namespace ArcadeHero2D.Core.Flow
{
    public static class SceneLoadFlow
    {
        public static string NextSceneName { get; private set; }

        public static void LoadViaPreload(string targetScene, string preloadScene = "Preload")
        {
            NextSceneName = targetScene;
            SceneManager.LoadScene(preloadScene);
        }

        public static string ConsumeTargetOr(string fallback)
        {
            var t = string.IsNullOrEmpty(NextSceneName) ? fallback : NextSceneName;
            NextSceneName = null;
            return t;
        }
    }
}