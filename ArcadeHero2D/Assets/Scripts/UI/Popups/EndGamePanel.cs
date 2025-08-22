using ArcadeHero2D.Core.Flow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArcadeHero2D.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public sealed class EndGamePanel : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text subtitle;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        [Header("Sorting")]
        [SerializeField] private bool forceTopmost = true;
        [SerializeField] private int sortingOrder = 5000; 

        System.Action _onRestart;
        System.Action _onMenu;

        private void Reset()
        {
            canvas = GetComponent<Canvas>();
            group = GetComponent<CanvasGroup>();
            raycaster = GetComponent<GraphicRaycaster>();
        }

        private void Awake()
        {
            if (!canvas)     canvas    = GetComponent<Canvas>();
            if (!group)      group     = GetComponent<CanvasGroup>();
            if (!raycaster)  raycaster = GetComponent<GraphicRaycaster>();

            if (forceTopmost)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.overrideSorting = true;
                canvas.sortingOrder = sortingOrder;
            }

            HideImmediate();
        }

        public void Show(GameResult result, string extra = null, System.Action onRestart = null, System.Action onMenu = null)
        {
            _onRestart = onRestart;
            _onMenu = onMenu;

            if (title)    title.text = result == GameResult.Victory ? "Победа!" : "Поражение";
            if (subtitle) subtitle.text = extra ?? "";

            gameObject.SetActive(true);

            if (raycaster) raycaster.enabled = true;

            group.alpha = 1f;
            group.blocksRaycasts = true;
            group.interactable = true;

            if (restartButton)
            {
                restartButton.interactable = true;
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(() => _onRestart?.Invoke());
            }
            if (menuButton)
            {
                menuButton.interactable = true;
                menuButton.onClick.RemoveAllListeners();
                menuButton.onClick.AddListener(() => _onMenu?.Invoke());
            }
        }

        public void HideImmediate()
        {
            if (!group) return;
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
            if (raycaster) raycaster.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
