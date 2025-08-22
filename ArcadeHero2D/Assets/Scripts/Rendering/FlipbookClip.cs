using UnityEngine;

namespace ArcadeHero2D.Rendering
{
    public enum FlipbookLoopMode { Loop, Once, PingPong }

    [CreateAssetMenu(fileName = "Flip_", menuName = "ArcadeHero2D/Flipbook Clip")]
    public sealed class FlipbookClip : ScriptableObject
    {
        [Header("Frames")]
        public Sprite[] frames;

        [Header("Playback")]
        [Min(1)] public int fps = 12;
        public FlipbookLoopMode loopMode = FlipbookLoopMode.Loop;
        public bool useUnscaledTime = false;

        public float FrameDuration => 1f / Mathf.Max(1, fps);
        public int FrameCount => frames != null ? frames.Length : 0;
    }
}